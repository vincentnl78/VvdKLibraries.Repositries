using System.Text.Json;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Serilog;
using VvdKRepositry.Repositries.Blob.Base;
using VvdKRepositry.Repositries.Contracts;
using VvdKRepositry.Repositries.Contracts.Blob.User;
using VvdKRepositry.Repositries.Contracts.Notifications.Creation;

namespace VvdKRepositry.Repositries.Blob.User;

public class UserBlobPersistence<TStorageParameterProvider>(
    TStorageParameterProvider idProvider,
    IAzureClientFactory<BlobServiceClient> factory,
    JsonSerializerOptions jsonSerializerOptions) 
    : BaseBlobPersistence(factory.CreateClient(idProvider.ServiceClientIdentifier), jsonSerializerOptions), IUserBlobPersistence<TStorageParameterProvider> 
    where TStorageParameterProvider : class, IBlobStorageParameterProvider
{
    public async  Task Handle(CreateUserPersistenceSetupNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            await InitializeAsync(idProvider.BlobContainerName);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Creating container");
        }
    }

    public async Task Handle(DeleteUserPersistenceNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            await DeleteContainerAsync(idProvider.BlobContainerName);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Cleanup");
        }
    }

    public  Task DeleteFileAsync(string filename, string? directory = null)
    {
        return base.DeleteFileAsync(idProvider.BlobContainerName,filename, directory);
    }

    public  Task ClearDirectoryAsync(string? directory = null)
    {
        return base.ClearDirectoryAsync(idProvider.BlobContainerName, directory);
    }

    public Task<List<string>> GetFilenamesAsync(string? directory = null)
    {
        return base.GetFilenamesAsync(idProvider.BlobContainerName, directory);
    }

    public  Task<bool> SaveStreamAsync(Stream openReadStream, string file, string? directory, string? leaseId = null,
        string contentType = "application/json")
    {
        return base.SaveStreamAsync(idProvider.BlobContainerName, openReadStream, file, directory, leaseId, contentType);
    }

    public  Task SaveTextAsync(string text, string filename, string? directory = null)
    {
        return base.SaveTextAsync(idProvider.BlobContainerName, text, filename, directory);
    }

    public  Task SaveObjectAsync<T>(T o, string filename, string? directory = null)
    {
        return base.SaveObjectAsync(idProvider.BlobContainerName, o, filename, directory);
    }

    public  Task<string> SavePotentiallyRenameImportFileAsync(Stream stream, string filename, string directory)
    {
        return base.SavePotentiallyRenameImportFileAsync(idProvider.BlobContainerName, stream, filename, directory);
    }

    public Task<string> AcquireLeaseAsync(string path, TimeSpan timeSpan, CancellationToken cancellationToken)
    {
        return base.AcquireLeaseAsync(idProvider.BlobContainerName,path,timeSpan, cancellationToken);
    }

    public  Task<bool> ReleaseLeaseAsync(string path, string? leaseId)
    {
        return base.ReleaseLeaseAsync(idProvider.BlobContainerName, path, leaseId);
    }

    public  Task<DateTimeOffset?> GetStartOfCurrentLeaseAsync(string path)
    {
        return base.GetStartOfCurrentLeaseAsync(idProvider.BlobContainerName, path);
    }

    public  Task<bool> ExistsAsync(string path)
    {
        return base.ExistsAsync(idProvider.BlobContainerName, path);
    }

    public  Task<bool> InitializeAsync()
    {
        return base.InitializeAsync(idProvider.BlobContainerName);
    }

    public  Task<bool> DeleteContainerAsync()
    {
        return base.DeleteContainerAsync(idProvider.BlobContainerName);
    }

    public  Task<Stream?> GetReadStreamAsync(string path, string? directory = null)
    {
        return base.GetReadStreamAsync(idProvider.BlobContainerName, path, directory);
    }

    public  Task<T?> GetAsync<T>(string filename, string? directory = null)
    {
        return base.GetAsync<T>(idProvider.BlobContainerName, filename, directory);
    }

    public  Task<string?> GetFileStringAsync(string filename, string directory)
    {
        return base.GetFileStringAsync(idProvider.BlobContainerName, filename, directory);
    }
}