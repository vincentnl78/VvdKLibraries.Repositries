using VvdKRepositry.Repositries.Contracts.Blob.General;
using VvdKRepositry.Repositries.Contracts.Notifications.Creation;

namespace VvdKRepositry.Repositries.UnitTests;

public class FakeBlobStore : IGeneralBlobPersistence
{
    private readonly Dictionary<string, byte[]> _store = new();
    public Task<byte[]> GetAsync(string key)
    {
        return Task.FromResult(_store[key]);
    }

    public Task SetAsync(string key, byte[] data)
    {
        _store[key] = data;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string key)
    {
        _store.Remove(key);
        return Task.CompletedTask;
    }

    public Task DeleteFileAsync(string container, string filename, string? directory = null)
    {
        throw new NotImplementedException();
    }

    public Task ClearDirectoryAsync(string container, string? directory = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SaveStreamAsync(string container, Stream openReadStream, string file, string? directory, string? leaseId = null,
        string contentType = "application/json")
    {
        throw new NotImplementedException();
    }

    public Task SaveTextAsync(string container, string text, string filename, string? directory = null)
    {
        throw new NotImplementedException();
    }

    public Task SaveObjectAsync<T>(string container, T o, string filename, string? directory = null)
    {
        throw new NotImplementedException();
    }

    public Task<string> SavePotentiallyRenameImportFileAsync(string container, Stream stream, string filename, string directory)
    {
        throw new NotImplementedException();
    }

    public Task<string> AcquireLease(string container, bool infinite, string path, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ReleaseLease(string container, string path, string? leaseId)
    {
        throw new NotImplementedException();
    }

    public Task<DateTimeOffset?> GetStartOfCurrentLeaseAsync(string container, string path)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(string container, string path)
    {
        throw new NotImplementedException();
    }

    public Task<bool> InitializeAsync(string container)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteContainerAsync(string container)
    {
        throw new NotImplementedException();
    }

    public Task<Stream?> GetReadStreamAsync(string container, string path, string? directory = null)
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetAsync<T>(string container, string filename, string? directory = null)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetFileStringAsync(string container, string filename, string directory)
    {
        throw new NotImplementedException();
    }

    public Task Handle(CreateUserPersistenceSetupNotification notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task Handle(DeleteUserPersistenceNotification notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}