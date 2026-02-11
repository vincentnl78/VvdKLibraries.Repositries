using VvdKRepositry.Repositries.Contracts;
using VvdKRepositry.Repositries.Contracts.Blob.User;

namespace VvdKRepositry.Repositries.Blob.User;

public abstract class UserBlobRepositry<TIdProvider>(IUserBlobPersistence<TIdProvider> userBlobPersistence)
    :  IUserBlobRepositry where TIdProvider : class, IBlobStorageParameterProvider
{
    public  Task DeleteFileAsync(string filename, string? directory = null)
    {
        return userBlobPersistence.DeleteFileAsync(filename, directory);
    }

    public  Task ClearDirectoryAsync(string? directory = null)
    {
        return userBlobPersistence.ClearDirectoryAsync(directory);
    }

    public Task<List<string>> GetFilenamesAsync(string? directory = null)
    {
        return userBlobPersistence.GetFilenamesAsync(directory); 
    }

    public  Task<bool> SaveStreamAsync(Stream openReadStream, string file, string? directory, string? leaseId = null,
        string contentType = "application/json")
    {
        return userBlobPersistence.SaveStreamAsync(openReadStream, file, directory, leaseId, contentType);
    }

    public  Task SaveTextAsync(string text, string filename, string? directory = null)
    {
        return userBlobPersistence.SaveTextAsync(text, filename, directory);
    }

    public  Task SaveObjectAsync<T>(T o, string filename, string? directory = null)
    {
        return userBlobPersistence.SaveObjectAsync(o, filename, directory);
    }

    public  Task<string> SavePotentiallyRenameImportFileAsync(Stream stream, string filename, string directory)
    {
        return userBlobPersistence.SavePotentiallyRenameImportFileAsync(stream, filename, directory);
    }

    public Task<string> AcquireLeaseAsync(string path, TimeSpan timeSpan, CancellationToken cancellationToken)
    {
        return userBlobPersistence.AcquireLeaseAsync(path,timeSpan,  cancellationToken);
    }

    public  Task<bool> ReleaseLeaseAsync(string path, string? leaseId)
    {
        return userBlobPersistence.ReleaseLeaseAsync(path, leaseId);
    }

    public  Task<DateTimeOffset?> GetStartOfCurrentLeaseAsync(string path)
    {
        return userBlobPersistence.GetStartOfCurrentLeaseAsync(path);
    }

    public  Task<bool> ExistsAsync(string path)
    {
        return userBlobPersistence.ExistsAsync(path);
    }

    public  Task<bool> InitializeAsync()
    {
        return userBlobPersistence.InitializeAsync();
    }

    public  Task<bool> DeleteContainerAsync()
    {
        return userBlobPersistence.DeleteContainerAsync();
    }

    public  Task<Stream?> GetReadStreamAsync(string path, string? directory = null)
    {
        return userBlobPersistence.GetReadStreamAsync(path, directory);
    }

    public  Task<T?> GetAsync<T>(string filename, string? directory = null)
    {
        return userBlobPersistence.GetAsync<T>(filename, directory);
    }

    public  Task<string?> GetFileStringAsync(string filename, string directory)
    {
        return userBlobPersistence.GetFileStringAsync(filename, directory);
    }
}