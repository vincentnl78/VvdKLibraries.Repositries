using System.Text.Json;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Serilog;
using VvdKRepositry.Repositries.Blob.Base;
using VvdKRepositry.Repositries.Contracts;
using VvdKRepositry.Repositries.Contracts.Blob.User;
using VvdKRepositry.Repositries.Contracts.Notifications.Creation;

namespace VvdKRepositry.Repositries.Blob.User;

public class UserBlobPersistence(
    IIdProvider idProvider,
    IAzureClientFactory<BlobServiceClient> factory,
    JsonSerializerOptions jsonSerializerOptions) 
    : BaseBlobPersistence(factory.CreateClient(idProvider.ServiceClientIdentifier), jsonSerializerOptions), IUserBlobPersistence
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
        return base.DeleteFileAsync(idProvider.TableName,filename, directory);
    }

    public  Task ClearDirectoryAsync(string? directory = null)
    {
        return base.ClearDirectoryAsync(idProvider.TableName, directory);
    }

    public  Task<bool> SaveStreamAsync(Stream openReadStream, string file, string? directory, string? leaseId = null,
        string contentType = "application/json")
    {
        return base.SaveStreamAsync(idProvider.TableName, openReadStream, file, directory, leaseId, contentType);
    }

    public  Task SaveTextAsync(string text, string filename, string? directory = null)
    {
        return base.SaveTextAsync(idProvider.TableName, text, filename, directory);
    }

    public  Task SaveObjectAsync<T>(T o, string filename, string? directory = null)
    {
        return base.SaveObjectAsync(idProvider.TableName, o, filename, directory);
    }

    public  Task<string> SavePotentiallyRenameImportFileAsync(Stream stream, string filename, string directory)
    {
        return base.SavePotentiallyRenameImportFileAsync(idProvider.TableName, stream, filename, directory);
    }

    public  Task<string> AcquireLease(bool infinite, string path, CancellationToken cancellationToken)
    {
        return base.AcquireLease(idProvider.TableName, infinite, path, cancellationToken);
    }

    public  Task<bool> ReleaseLease(string path, string? leaseId)
    {
        return base.ReleaseLease(idProvider.TableName, path, leaseId);
    }

    public  Task<DateTimeOffset?> GetStartOfCurrentLeaseAsync(string path)
    {
        return base.GetStartOfCurrentLeaseAsync(idProvider.TableName, path);
    }

    public  Task<bool> ExistsAsync(string path)
    {
        return base.ExistsAsync(idProvider.TableName, path);
    }

    public  Task<bool> InitializeAsync()
    {
        return base.InitializeAsync(idProvider.TableName);
    }

    public  Task<bool> DeleteContainerAsync()
    {
        return base.DeleteContainerAsync(idProvider.TableName);
    }

    public  Task<Stream?> GetReadStreamAsync(string path, string? directory = null)
    {
        return base.GetReadStreamAsync(idProvider.TableName, path, directory);
    }

    public  Task<T?> GetAsync<T>(string filename, string? directory = null)
    {
        return base.GetAsync<T>(idProvider.TableName, filename, directory);
    }

    public  Task<string?> GetFileStringAsync(string filename, string directory)
    {
        return base.GetFileStringAsync(idProvider.TableName, filename, directory);
    }
}