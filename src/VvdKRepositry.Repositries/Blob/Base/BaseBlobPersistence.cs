using System.Text;
using System.Text.Json;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Serilog;
using VvdKRepositry.Repositries.Contracts.Blob.Base;

namespace VvdKRepositry.Repositries.Blob.Base;

public abstract class BaseBlobPersistence(BlobServiceClient client, JsonSerializerOptions jsonSerializerOptions)
    : IBaseBlobPersistence,IAsyncDisposable 
{
    public virtual async Task DeleteFileAsync(string containername, string filename, string? directory)
    {
        await client.GetBlobContainerClient(containername).GetBlobClient(MakePath(filename, directory)).DeleteAsync();
    }

    public async Task ClearDirectoryAsync(string containername, string? directory = null)
    {
        var container = client.GetBlobContainerClient(containername);
        await foreach (var blobItem in container.GetBlobsAsync(
                           BlobTraits.None,
                           BlobStates.None,
                           directory,
                           CancellationToken.None))
            await container.GetBlobClient(blobItem.Name).DeleteAsync();
    }

    public async Task<List<string>> GetFilenamesAsync(string containername, string? directory = null)
    {
        List<string> filenames = [];
        var container = client.GetBlobContainerClient(containername);
        await foreach (var blobItem in container.GetBlobsAsync(
                           BlobTraits.None,
                            BlobStates.None,
                           directory,
                           CancellationToken.None
                           ))
        {
            filenames.Add(blobItem.Name);
        }
        return filenames;
    }

    public virtual async Task<Stream?> GetReadStreamAsync(string containername, string filename,
        string? directory = null)
    {
        return await GetReadStreamAsync(GetBlobClient(containername, MakePath(filename, directory)));
    }

    public virtual async Task<bool> SaveStreamAsync(string containername, Stream openReadStream, string file,
        string? directory,
        string? leaseId = null, string? contentType = null)
    {
        var blob = GetBlobClient(containername, MakePath(file, directory));
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


    public virtual async Task<bool> ExistsAsync(string containername, string path)
    {
        var blob = GetBlobClient(containername, path);
        return await blob.ExistsAsync();
    }

    public async Task<T?> GetAsync<T>(string containername, string filename, string? directory = null)
    {
        var stream = await GetReadStreamAsync(containername, MakePath(filename, directory));
        if (stream == null) return default;

        return JsonSerializer.Deserialize<T>(stream, jsonSerializerOptions);
    }

    public async Task SaveTextAsync(string containername, string text, string filename, string? directory)
    {
        var byteArray = Encoding.ASCII.GetBytes(text);
        using MemoryStream ms = new(byteArray);
        await SaveStreamAsync(containername, ms, filename, directory);
    }

    public async Task<string?> GetFileStringAsync(string containername, string filename, string directory)
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

    public async Task SaveObjectAsync<T>(string containername, T o, string filename, string? directory = null)
    {
        var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, o);
        await SaveStreamAsync(containername, stream, filename, directory);
    }

    public async Task<string> SavePotentiallyRenameImportFileAsync(string containername, Stream stream, string filename,
        string directory)
    {
        var count = 1;

        var filenameToBeUsed = filename;
        while (true)
        {
            if (await ExistsAsync(containername, MakePath(filenameToBeUsed, directory)))
            {
                filenameToBeUsed = CreateFollowUpFilename(filename, count++);
                continue;
            }

            await SaveStreamAsync(containername, stream, filenameToBeUsed, directory);
            break;
        }

        return filenameToBeUsed;
    }

    private async Task<BlobProperties> GetBlobPropertiesAsync(string containername, string path)
    {
        var blob = GetBlobClient(containername, path);
        return (await blob.GetPropertiesAsync()).Value;
    }

    private string CreateFollowUpFilename(string filename, int count)
    {
        var ext = Path.GetExtension(filename);
        filename = Path.GetFileNameWithoutExtension(filename);
        return $"{filename}({count}){ext}";
    }

    private BlobClient GetBlobClient(string containername, string path)
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

    #region Leases

    private CancellationTokenSource? _renewalCts;
    private Task? _renewalTask;
   
    public async Task<string> AcquireLeaseAsync(string container,string path,TimeSpan timespan, CancellationToken cancellationToken)
    {
        var blobClient = GetBlobClient(container, path);
        var leaseClient = blobClient.GetBlobLeaseClient();
        var leaseResponse = await leaseClient.AcquireAsync(
            timespan<=TimeSpan.FromSeconds(60)
            ? timespan: TimeSpan.FromSeconds(60), // Azure Storage has a max lease time of 60 seconds
            cancellationToken: cancellationToken);
        if (timespan > TimeSpan.FromSeconds(60))
        {
            Log.Information("Long lease started");
            _renewalCts = new CancellationTokenSource();
            var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _renewalCts.Token).Token;
            _renewalTask = RenewLeasePeriodicallyAsync(blobClient, leaseResponse.Value.LeaseId, linkedToken);    
        }
        else
        {
            Log.Information("Short lease started");
        }
        return leaseResponse.Value.LeaseId;
    }

    public async Task<bool> ReleaseLeaseAsync(string container,string path,string? leaseId)
    {
        var blobClient = GetBlobClient(container, path);

        // ReSharper disable once MethodHasAsyncOverload
        _renewalCts?.Cancel();
        
        if (_renewalTask != null)
        {
            try { await _renewalTask; } catch (TaskCanceledException) { }
        }

        if (leaseId != null)
        {
            var leaseClient = blobClient.GetBlobLeaseClient(leaseId);
            await leaseClient.ReleaseAsync();    
        }
        else
        {
            var leaseClient = blobClient.GetBlobLeaseClient();
            await leaseClient.BreakAsync();
            Log.Information("Breaking lease for {Path}", path);
        }
        return true;
    }

    private async Task RenewLeasePeriodicallyAsync(BlobClient blobClient, string leaseId, CancellationToken token)
    {
        var leaseClient = blobClient.GetBlobLeaseClient(leaseId);
        try
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(50), token);
                try
                {
                    await leaseClient.RenewAsync(cancellationToken: token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lease renewal failed: {ex.Message}");
                }
            }
        }
        catch (TaskCanceledException) { }
    }

    public async ValueTask DisposeAsync()
    {
        if (_renewalCts is not null)
        {
            // ReSharper disable once MethodHasAsyncOverload
            _renewalCts?.Cancel();
            if (_renewalTask != null)
            {
                try { await _renewalTask; } catch (TaskCanceledException) { }
            }
        }
        _renewalCts?.Dispose();
        _renewalCts = null;
    }
    
   

    

    public async Task<DateTimeOffset?> GetStartOfCurrentLeaseAsync(string containername, string path)
    {
        var properties = await GetBlobPropertiesAsync(containername, path);
        if (properties is { LeaseState: LeaseState.Leased, LeaseStatus: LeaseStatus.Locked })
            return properties.LastModified;

        return null;
    }

    #endregion

    #region Lifecycle

    public virtual async Task<bool> InitializeAsync(string containername)
    {
        var containerClient = client.GetBlobContainerClient(containername);
        try
        {
            var result = await containerClient.CreateIfNotExistsAsync();
            if (result == null)//null check must stay https://github.com/Azure/azure-sdk-for-net/issues/9758
            {
                Log.Information("Creating container {ContainerName}, already exists - null return", containername);
                return false;
            } 
                
            var statusCode = result.GetRawResponse().Status;
            switch (statusCode)
            {
                case 409:
                    Log.Information("Creating container {ContainerName}, already exists - 409 return", containername);
                    break;
                case 201 or 204:
                    Log.Information("Blob container created: {ContainerName}", containername);
                    break;
            }

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Creating container");
            throw;
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
}