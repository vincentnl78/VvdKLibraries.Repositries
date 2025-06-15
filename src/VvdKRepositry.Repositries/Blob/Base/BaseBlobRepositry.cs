using VvdKRepositry.Repositries.Contracts.Blob.Base;

namespace VvdKRepositry.Repositries.Blob.Base;

public abstract class BaseBlobRepositry(IBaseBlobPersistence persistence):IBlobRepositry
{
    protected abstract string ContainerName { get; }

    public  Task DeleteFileAsync(string filename, string? directory = null)
    {
        return persistence.DeleteFileAsync(ContainerName,filename, directory);
    }

    public  Task ClearDirectoryAsync(string? directory = null)
    {
        return persistence.ClearDirectoryAsync(ContainerName, directory);
    }

    public  Task<bool> SaveStreamAsync(Stream openReadStream, string file, string? directory, string? leaseId = null,
        string contentType = "application/json")
    {
        return persistence.SaveStreamAsync(ContainerName,openReadStream,file,directory,leaseId,contentType);
    }

    public  Task SaveTextAsync(string text, string filename, string? directory = null)
    {
        return persistence.SaveTextAsync(ContainerName,text,filename,directory);
    }

    public  Task SaveObjectAsync<T>(T o, string filename, string? directory = null)
    {
        return persistence.SaveObjectAsync(ContainerName, o, filename, directory);
    }

    public  Task<string> SavePotentiallyRenameImportFileAsync(Stream stream, string filename, string directory)
    {
        return persistence.SavePotentiallyRenameImportFileAsync(ContainerName,stream,filename,directory);
    }

    public  Task<string> AcquireLease(bool infinite, string path, CancellationToken cancellationToken)
    {
        return persistence.AcquireLease(ContainerName,infinite,path,cancellationToken);
    }

    public  Task<bool> ReleaseLease(string path, string? leaseId)
    {
        return persistence.ReleaseLease(ContainerName,path,leaseId);
    }

    public  Task<DateTimeOffset?> GetStartOfCurrentLeaseAsync(string path)
    {
        return persistence.GetStartOfCurrentLeaseAsync(ContainerName,path);
    }

    public  Task<bool> ExistsAsync(string path)
    {
        return persistence.ExistsAsync(ContainerName,path);
    }

    public  Task<bool> InitializeAsync()
    {
        return persistence.InitializeAsync(ContainerName);
    }

    public  Task<bool> DeleteContainerAsync()
    {
        return persistence.DeleteContainerAsync(ContainerName);
    }

    public  Task<Stream?> GetReadStreamAsync(string path, string? directory = null)
    {
        return persistence.GetReadStreamAsync(ContainerName,path,directory);
    }

    public  Task<T?> GetAsync<T>(string filename, string? directory = null)
    {
        return persistence.GetAsync<T>(ContainerName,filename,directory);
    }

    public  Task<string?> GetFileStringAsync(string filename, string directory)
    {
        return persistence.GetFileStringAsync(ContainerName,filename,directory);
    }
    
    
}