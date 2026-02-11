namespace VvdKRepositry.Repositries.Contracts.Blob.Base;

public interface IBaseBlobPersistence
{
    Task DeleteFileAsync(string container, string filename, string? directory = null);
    Task ClearDirectoryAsync(string container,string? directory = null);
    Task<List<string>> GetFilenamesAsync(string container,string? directory = null);

    Task<bool> SaveStreamAsync(string container,Stream openReadStream, string file, string? directory, string? leaseId = null, string contentType = "application/json");

    Task SaveTextAsync(string container,string text, string filename, string? directory = null);
    Task SaveObjectAsync<T>(string container,T o, string filename, string? directory = null);
    Task<string> SavePotentiallyRenameImportFileAsync(string container,Stream stream, string filename, string directory);


    Task<string> AcquireLeaseAsync(string container,string path,TimeSpan timespan, CancellationToken cancellationToken);
    Task<bool> ReleaseLeaseAsync(string container,string path,string? leaseId);
    Task<DateTimeOffset?> GetStartOfCurrentLeaseAsync(string container,string path);

    Task<bool> ExistsAsync(string container,string path);
    Task<bool> InitializeAsync(string container);
    Task<bool> DeleteContainerAsync(string container);


    Task<Stream?> GetReadStreamAsync(string container,string path, string? directory = null);
    Task<T?> GetAsync<T>(string container,string filename, string? directory = null);
    Task<string?> GetFileStringAsync(string container,string filename, string directory);
    
}