namespace VvdKRepositry.Repositries.Contracts.Blob.Base;

public interface IBlobRepositry
{
    Task DeleteFileAsync(string filename, string? directory = null);
    Task ClearDirectoryAsync(string? directory = null);

    Task<bool> SaveStreamAsync(Stream openReadStream, string file, string? directory, string? leaseId = null, string contentType = "application/json");

    Task SaveTextAsync(string text, string filename, string? directory = null);
    Task SaveObjectAsync<T>(T o, string filename, string? directory = null);
    Task<string> SavePotentiallyRenameImportFileAsync(Stream stream, string filename, string directory);


    Task<string> AcquireLease(bool infinite, string path, CancellationToken cancellationToken);
    Task<bool> ReleaseLease(string path,string? leaseId);
    Task<DateTimeOffset?> GetStartOfCurrentLeaseAsync(string path);

    Task<bool> ExistsAsync(string path);
    Task<bool> InitializeAsync();
    Task<bool> DeleteContainerAsync();


    Task<Stream?> GetReadStreamAsync(string path, string? directory = null);
    Task<T?> GetAsync<T>(string filename, string? directory = null);
    Task<string?> GetFileStringAsync(string filename, string directory);
}