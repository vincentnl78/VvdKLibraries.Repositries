using System.Text.Json;
using VvdKRepositry.Repositries.Contracts;
using VvdKRepositry.Repositries.Contracts.Blob.User;

namespace VvdKRepositry.Repositries.Blob.User;

public abstract class JsonRepositrySingleClassBacking<T,TIdProvider>(
    IUserBlobPersistence<TIdProvider> userBlobPersistence,
    JsonSerializerOptions jsonSerializerOptions)
    :JsonRepositryBaseBacking<T,TIdProvider>(userBlobPersistence,jsonSerializerOptions), IReadWriteSingleClass<T>
    where T : new() where TIdProvider : class, IBlobStorageParameterProvider
{
    // ReSharper disable once MemberCanBePrivate.Global
    protected override T Content { get; set; } = new T();
    public T Instance
    {
        get => Content;
        set => Content = value; 
    }
}