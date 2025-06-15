using System.Text.Json;
using VvdKRepositry.Repositries.Contracts.Blob.User;

namespace VvdKRepositry.Repositries.Blob.User;

public abstract class JsonRepositryDictionaryBacking<TKey,T>(
    IUserBlobPersistence persistence,
    JsonSerializerOptions jsonSerializerOptions)
    : JsonRepositryBaseBacking<Dictionary<TKey,T>>(persistence, jsonSerializerOptions),
        IReadDictionaryRepository<TKey,T>,
        IWriteRepository<TKey,T>
    where T: IId<TKey>
    where TKey : struct
{
    private Dictionary<TKey,T>? _content;
    protected override Dictionary<TKey,T> Content
    {
        get => _content ??= [];
        set
        {
            _content = value;
            Dirty = true;
        }
    }

    public IReadOnlyDictionary<TKey, T> Dictionary => Content;

    public IEnumerable<T> All => Content.Values;
    public bool TryGet(TKey id, out T value)
    {
        if (Content.TryGetValue(id, out var entity))
        {
            value = entity;
            return true;
        }
        
        value = default!;
        return false;
    }

    public virtual T Add(T entity)
    {
        Content.Add(entity.Id, entity);
        Dirty = true;
        return entity;
    }
    
    public virtual void Update(T entity)
    {
        Content[entity.Id] = entity;
        Dirty = true;
    }

    public virtual void Update(IEnumerable<T> entity)
    {
        foreach (var item in entity)
        {
            Content[item.Id] = item;
        }
        Dirty = true;
    }

    public void Remove(TKey id)
    {
        if (Content.Remove(id))
        {
            Dirty = true;
        }
    }
}
