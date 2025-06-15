using System.Text.Json;
using VvdKRepositry.Repositries.Contracts.Blob.User;

namespace VvdKRepositry.Repositries.Blob.User;

public abstract class JsonRepositryIntDictionaryBacking<T>(
    IUserBlobPersistence persistence,
    JsonSerializerOptions jsonSerializerOptions)
    : JsonRepositryDictionaryBacking<int,T>(persistence, jsonSerializerOptions)
    where T : EntityWithId<int>
{
    public override T Add(T entity)
    {
        var newEntity = entity.Id==0
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
}

