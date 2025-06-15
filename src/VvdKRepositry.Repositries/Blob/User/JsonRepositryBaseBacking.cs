using System.Text.Json;
using Serilog;
using VvdKRepositry.Repositries.Contracts.Blob.Base;
using VvdKRepositry.Repositries.Contracts.Blob.User;
using VvdKRepositry.Repositries.Contracts.Notifications.Repositry;

namespace VvdKRepositry.Repositries.Blob.User;

public abstract class JsonRepositryBaseBacking<T>(
    IUserBlobPersistence userBlobPersistence,
    JsonSerializerOptions jsonSerializerOptions)
    :UserBlobRepositry(userBlobPersistence), IBaseBacking<T>,IRepositryWorkNotifications,ILockableByLease
    where T : new()
{
    private string? _leaseId;
    private JsonSerializerOptions JsonSerializerOptions { get; } = jsonSerializerOptions;

    protected abstract string PackageName { get; }
    // ReSharper disable once MemberCanBePrivate.Global
    public bool IsLoaded { get; private set; }

    protected abstract T Content { get; set; }
    
    // protected TInterface Content
    // {
    //     get => _content ??= new TImplementation();
    //     set
    //     {
    //         _content = value;
    //         Dirty = true;
    //     }
    // }

    public bool Dirty { get; set; }


    public virtual async Task LoadAsync()
    {
        try
        {
            if (!IsLoaded)
            {
                T? loaded = default;
                //if (await userDataStreamPersistence.ExistsAsync(PackageName))
                //{
                var stream = await GetReadStreamAsync(PackageName);
                if (stream != null)
                {
                    stream.Position = 0;
                    loaded = await JsonSerializer.DeserializeAsync<T>(stream, JsonSerializerOptions);
                }
                //}

                Content = loaded ?? new T();
                Dirty = false; //if it's new - no need to save unless changes
            }
        }
        catch (Exception ex)
        {
            if (await ExistsAsync(PackageName))
                Log.Fatal(ex, "loading settings");
            throw;
        }
        finally
        {
            IsLoaded = true;
        }
    }
    
    public Task Handle(UnloadRepositryNotification notification, CancellationToken cancellationToken)
    {
        Content = new T(); //skip the Dirty Setting
        Dirty = false;
        _leaseId = null;
        IsLoaded = false;
        return Task.CompletedTask;
    }

    public Task Handle(CommitChangesRepositryNotification notification, CancellationToken cancellationToken)
    {
        return SaveAsync();
    }
    
    private async Task SaveAsync()
    {
        if (!Dirty) return;
        using MemoryStream stream = new();
        await using Utf8JsonWriter writer = new(stream);
        JsonSerializer.Serialize(writer, Content, JsonSerializerOptions);
        await SaveStreamAsync(stream, PackageName, null, _leaseId);

        Dirty = false;
    }

    public async Task<bool> AcquireLease(CancellationToken cancellationToken = default)
    {
        _leaseId = await AcquireLease(true, PackageName, cancellationToken);
        return _leaseId != null;
    }

    public async Task<bool> ReleaseLease(bool force)
    {
        if (!force && _leaseId == null)
        {
            Log.Warning("No leaseId to release for {Filename}", PackageName);
            return false;
        }

        bool success;
        if (_leaseId != null)
        {
            success = await ReleaseLease(PackageName, _leaseId);
            if (success)
            {
                //Log.Verbose("Released lease {LeaseId} for {Filename}", _leaseId, PackageName);
            }
            else
            {
                Log.Warning("Failed to release lease {LeaseId} for {Filename}", _leaseId, PackageName);
            }

            _leaseId = null;
        }
        else
        {
            Log.Verbose("Breaking lease for {Filename}", PackageName);
            success = await ReleaseLease(PackageName, null);
        }

        return success;
    }

    public Task<DateTimeOffset?> GetStartOfCurrentLeaseAsync()
    {
        return GetStartOfCurrentLeaseAsync(PackageName);
    }
}