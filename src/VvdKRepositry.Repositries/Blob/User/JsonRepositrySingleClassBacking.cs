using System.Text.Json;
using VvdKRepositry.Repositries.Contracts.Blob.User;

namespace VvdKRepositry.Repositries.Blob.User;

public abstract class JsonRepositrySingleClassBacking<T>(
    IUserBlobPersistence userBlobPersistence,
    JsonSerializerOptions jsonSerializerOptions)
    :JsonRepositryBaseBacking<T>(userBlobPersistence,jsonSerializerOptions), IReadWriteSingleClass<T>
    where T : new()
{
    // ReSharper disable once MemberCanBePrivate.Global
    protected override T Content { get; set; } = new T();
    public T Instance
    {
        get => Content;
        set => Content = value;
    }
}