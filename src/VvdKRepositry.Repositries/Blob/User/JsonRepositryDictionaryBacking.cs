using System.Text.Json;
using VvdKRepositry.Repositries.Contracts.Blob.User;

namespace VvdKRepositry.Repositries.Blob.User;

public abstract class JsonRepositryDictionaryBacking<T>(
    IUserBlobPersistence persistence,
    JsonSerializerOptions jsonSerializerOptions)
    : JsonRepositryBaseBacking<Dictionary<int,T>>(persistence, jsonSerializerOptions),
        IReadDictionaryRepository<T>,
        IWriteRepository<T>
    where T : EntityWithIntId
{
    private Dictionary<int,T>? _content;
    protected override Dictionary<int,T> Content
    {
        get => _content ??= [];
        set
        {
            _content = value;
            Dirty = true;
        }
    }

    public IReadOnlyDictionary<int, T> All => Content;

    public T? GetById(int id)
    {
        return Content.TryGetValue(id, out var entity) ? entity : null;
    }

    public virtual T Add(T entity)
    {
        var newEntity = entity.Id == 0
            ? entity with
            {
                Id = Content.Any()
                    ? Content.Keys.Max() + 1
                    : 1
            }
            : entity;
        Content.Add(newEntity.Id, newEntity);
        Dirty = true;
        return newEntity;
    }

    public virtual void Update(T entity)
    {
        Content[entity.Id] = entity;
        Dirty = true;
    }

    public void Remove(int id)
    {
        if (Content.Remove(id))
        {
            Dirty = true;
        }
    }
}