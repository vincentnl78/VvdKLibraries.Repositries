using System.Text.Json;
using VvdKRepositry.Repositries.Contracts.Blob.User;

namespace VvdKRepositry.Repositries.Blob.User;

public abstract class JsonRepositryListBacking<T>(
    IUserBlobPersistence persistence,
    JsonSerializerOptions jsonSerializerOptions)
    : JsonRepositryBaseBacking<List<T>>(persistence, jsonSerializerOptions),
        IReadRepository<T>,
        IWriteRepository<T>
    where T : EntityWithIntId
{
    public IEnumerable<T> All => Content;

    private List<T>? _content;
    
    protected override List<T> Content
    {
        get => _content ??= [];
        set
        {
            _content = value;
            Dirty = true;
        }
    }

    public T? GetById(int id)
    {
        return Content.Find(i => i.Id == id);
    }

    public virtual T Add(T entity)
    {
        var newEntity = entity.Id == 0
            ? entity with
            {
                Id = Content.Any()
                    ? Content.Max(i => i.Id) + 1
                    : 1
            }
            : entity;
        Content.Add(newEntity);
        Dirty = true;
        return newEntity;
    }

    public virtual void Update(T entity)
    {
        Content.RemoveAll(e => e.Id == entity.Id);
        Content.Add(entity);
        Dirty = true;
    }

    public void Remove(int id)
    {
        Content.RemoveAll(e => e.Id == id);
        Dirty = true;
    }
}