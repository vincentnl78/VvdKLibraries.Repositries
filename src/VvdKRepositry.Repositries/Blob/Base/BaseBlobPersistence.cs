using System.Text;
using System.Text.Json;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Serilog;
using VvdKRepositry.Repositries.Contracts.Blob.Base;

namespace VvdKRepositry.Repositries.Blob.Base;

public abstract class BaseBlobPersistence(BlobServiceClient client, JsonSerializerOptions jsonSerializerOptions) : IBaseBlobPersistence
{
    public virtual async Task DeleteFileAsync(string containername, string filename, string? directory)
    {
        await client.GetBlobContainerClient(containername).GetBlobClient(MakePath(filename, directory)).DeleteAsync();
    }

    public async Task ClearDirectoryAsync(string containername,string? directory = null)
    {
        var container = client.GetBlobContainerClient(containername);
        await foreach (var blobItem in container.GetBlobsAsync(prefix: directory))
        {
            await container.GetBlobClient(blobItem.Name).DeleteAsync();
        }
    }
    public virtual async Task<Stream?> GetReadStreamAsync(string containername,string filename, string? directory = null)
    {
        return await GetReadStreamAsync(GetBlobClient(containername,MakePath(filename, directory)));
    }

    public virtual async Task<bool> SaveStreamAsync(string containername,Stream openReadStream, string file, string? directory,
        string? leaseId = null, string? contentType = null)
    {
        var blob = GetBlobClient(containername,MakePath(file, directory));
        var blobUploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            }
        };
        if (leaseId is null) return await SaveAsync(blob, openReadStream, blobUploadOptions);


        blobUploadOptions.Conditions = new BlobRequestConditions
        {
            LeaseId = leaseId
        };
        return await SaveAsync(blob, openReadStream, blobUploadOptions);
    }
    
    #region Leases

    public async Task<string> AcquireLease(string containername,bool infinite, string path,
        CancellationToken cancellationToken)
    {
        var blob = GetBlobClient(containername,path);
        var leaseBlobClient = blob.GetBlobLeaseClient();
        var timespan = infinite
            ? BlobLeaseClient.InfiniteLeaseDuration
            : TimeSpan.FromSeconds(60);
        try
        {
            Response<BlobLease> blobLeaseResponse =
                await leaseBlobClient.AcquireAsync(timespan, null, cancellationToken);
            if (blobLeaseResponse != null)
            {
                Log.ForContext<BaseBlobPersistence>().Verbose("Acquired lease {Path}", path);
                return blobLeaseResponse.Value.LeaseId;
            }

            Log.ForContext<BaseBlobPersistence>().Error("Could not get lease {Path}", path);
        }
        catch (RequestFailedException ex)
        {
            Log.ForContext<BaseBlobPersistence>().Error(ex, "Could not get lease {Path}", path);
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Could not get lease {Path}", path);
            throw;
        }

        throw new Exception($"Could not get lease {path} - unknown error");
    }

    public async Task<bool> ReleaseLease(string containername,string path, string? leaseId)
    {
        var blob = GetBlobClient(containername,path);
        var leaseBlobClient = blob.GetBlobLeaseClient(leaseId);
        try
        {
            if (leaseId == null)
            {
                var blobBreakLeaseResponse = await leaseBlobClient.BreakAsync();
                Log.ForContext<BaseBlobPersistence>().Verbose("Breaking lease for {Path}", path);
                return blobBreakLeaseResponse != null;
            }

            var blobLeaseResponse = await leaseBlobClient.ReleaseAsync();
            Log.ForContext<BaseBlobPersistence>().Verbose("Released lease {Lease} for {Path}", leaseId, path);
            return blobLeaseResponse != null;
        }
        catch (Exception ex)
        {
            Log.ForContext<BaseBlobPersistence>().Error(ex, "releasing lease failed, leaseId:{Lease}, {ExMessage}", leaseId,
                ex.Message);
        }

        return false;
    }

    public async Task<DateTimeOffset?> GetStartOfCurrentLeaseAsync(string containername,string path)
    {
        var properties = await GetBlobPropertiesAsync(containername,path);
        if (properties is { LeaseState: LeaseState.Leased, LeaseStatus: LeaseStatus.Locked })
        {
            return properties.LastModified;
        }

        return null;
    }

    #endregion


    public virtual async Task<bool> ExistsAsync(string containername,string path)
    {
        var blob = GetBlobClient(containername,path);
        return await blob.ExistsAsync();
    }

    #region Lifecycle
    public virtual async Task<bool> InitializeAsync(string containername)
    {
        var containerClient = client.GetBlobContainerClient(containername);
        try
        {
            var result = await containerClient.CreateIfNotExistsAsync();
            int statusCode = result.GetRawResponse().Status;
            
            if(statusCode ==409)
            {
                Log.Information("Creating container {ContainerName}, already exists", containername);
                return false;
            }
            
            if (statusCode is 201 or 204)
            {
                Log.Information("Blob container created: {ContainerName}", containername);
            }
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Creating container");
            await Task.Delay(5000);
            await containerClient.CreateIfNotExistsAsync();
            return false;
        }
    }

    public virtual async Task<bool> DeleteContainerAsync(string containername)
    {
        var container = client.GetBlobContainerClient(containername);
        await container.DeleteAsync();
        Log.Information("Blob container deleted: {ContainerName}", containername);
        return true;
    }
    
    #endregion

    public async Task<T?> GetAsync<T>(string containername,string filename, string? directory = null)
    {
        var stream = await GetReadStreamAsync( containername, MakePath(filename, directory));
        if (stream == null) return default;

        return JsonSerializer.Deserialize<T>(stream, jsonSerializerOptions);
    }

    public async Task SaveTextAsync(string containername,string text, string filename, string? directory)
    {
        var byteArray = Encoding.ASCII.GetBytes(text);
        using MemoryStream ms = new(byteArray);
        await SaveStreamAsync(containername,ms, filename, directory);
    }

    public async Task<string?> GetFileStringAsync(string containername,string filename, string directory)
    {
        var stream = await GetReadStreamAsync(filename, directory);
        if (stream != null)
        {
            StreamReader reader = new(stream);
            var text = await reader.ReadToEndAsync();
            return text;
        }

        return null;
    }

    public async Task SaveObjectAsync<T>(string containername,T o, string filename, string? directory = null)
    {
        var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, o);
        await SaveStreamAsync(containername,stream, filename, directory);
    }

    public async Task<string> SavePotentiallyRenameImportFileAsync(string containername,Stream stream, string filename, string directory)
    {
        var count = 1;

        var filenameToBeUsed = filename;
        while (true)
        {
            if (await ExistsAsync(containername,MakePath(filenameToBeUsed, directory)))
            {
                filenameToBeUsed = CreateFollowUpFilename(filename, count++);
                continue;
            }

            await SaveStreamAsync(containername,stream, filenameToBeUsed, directory);
            break;
        }

        return filenameToBeUsed;
    }

    private async Task<BlobProperties> GetBlobPropertiesAsync(string containername, string path)
    {
        var blob = GetBlobClient(containername,path);
        return (await blob.GetPropertiesAsync()).Value;
    }

    private string CreateFollowUpFilename(string filename, int count)
    {
        var ext = Path.GetExtension(filename);
        filename = Path.GetFileNameWithoutExtension(filename);
        return $"{filename}({count}){ext}";
    }

    private BlobClient GetBlobClient(string containername,string path)
    {
        return client.GetBlobContainerClient(containername).GetBlobClient(path);
    }

    private async Task<Stream?> GetReadStreamAsync(BlobClient blobClient)
    {
        Stream stream = new MemoryStream();
        if (await blobClient.ExistsAsync())
        {
            await blobClient.DownloadToAsync(stream);
            stream.Position = 0;
            return stream;
        }

        return null;
    }

    private async Task<bool> SaveAsync(BlobClient blobClient, Stream openReadStream,
        BlobUploadOptions? blobUploadOptions = null)
    {
        try
        {
            blobUploadOptions ??= new BlobUploadOptions();
            openReadStream.Position = 0;
            await blobClient.UploadAsync(openReadStream, blobUploadOptions);
            return true;
        }
        catch (RequestFailedException ex)
        {
            if (ex.ErrorCode == "LeaseLost" && blobUploadOptions is not null)
            {
                blobUploadOptions.Conditions.LeaseId = null;
                openReadStream.Position = 0;
                await blobClient.UploadAsync(openReadStream, blobUploadOptions);
                return true;
            }

            return false;
        }
    }

    private string MakePath(string filename, string? directory)
    {
        return directory == null
            ? filename
            : directory + "/" + filename;
    }
}