using System.Text.Json;
using VvdKRepositry.Repositries.Contracts.Blob.User;

namespace VvdKRepositry.Repositries.Blob.User;

public abstract class JsonRepositryDualTypeDictionaryBacking<T,T1,T2>(
    IUserBlobPersistence persistence,
    JsonSerializerOptions jsonSerializerOptions)
    : JsonRepositryBaseBacking<Dictionary<int,T>>(persistence, jsonSerializerOptions),
        IReadDualTypeDictionaryRepository<T,T1,T2>,
        IWriteRepository<T>
    where T : EntityWithIntId
    where T1:T
    where T2:T
{

    private Dictionary<int,T1>? _contentType1;
    private Dictionary<int,T2>? _contentType2;

    public IReadOnlyDictionary<int, T1> T1All => _contentType1 ?? [];
    public IReadOnlyDictionary<int, T2> T2All => _contentType2 ?? [];
    protected override Dictionary<int,T> Content
    {
        get
        {
            Dictionary<int, T> result = new();
            
            if(_contentType1 !=null)
                foreach (var kv in _contentType1)
                    result[kv.Key] = kv.Value; // T1 → T

            if(_contentType2 !=null)
                foreach (var kv in _contentType2)
                    result[kv.Key] = kv.Value; // T2 → T, overwrites if key exists

            return result;
        }
        set
        {
            _contentType1 = value.Values.OfType<T1>().ToDictionary(x => x.Id, x => x);
            _contentType2 = value.Values.OfType<T2>().ToDictionary(x => x.Id, x => x);
            Dirty = true;
        }
    }

    public virtual T Add(T entity)
    {
        if (_contentType1 == null || _contentType2 == null)
            throw new InvalidOperationException("Content dictionaries are not initialized. Ensure you have loaded the content before adding entities.");
        
        var newEntity = entity.Id == 0
            ? entity with
            {
                Id = Math.Max(
                   _contentType1.Keys.Max(),
                     _contentType2.Keys.Max()
                )+1,
            }
            : entity;
        
        switch (entity)
        {
            case T1 t1:
                _contentType1[entity.Id] = t1;
                break;
            case T2 t2:
                _contentType2[entity.Id] = t2;
                break;
            default:
                throw new ArgumentException($"Unsupported type {typeof(T)} for dual type repository.");
        }
        Dirty = true;
        return newEntity;
    }

    public virtual void Update(T entity)
    {
        if (_contentType1 == null || _contentType2 == null)
            throw new InvalidOperationException("Content dictionaries are not initialized. Ensure you have loaded the content before updating entities.");
        
        switch (entity)
        {
            case T1 t1:
                _contentType1[entity.Id] = t1;
                break;
            case T2 t2:
                _contentType2[entity.Id] = t2;
                break;
            default:
                throw new ArgumentException($"Unsupported type {typeof(T)} for dual type repository.");
        }
        Dirty = true;
    }

    public void Remove(int id)
    {
        if (_contentType1 == null || _contentType2 == null)
            throw new InvalidOperationException("Content dictionaries are not initialized. Ensure you have loaded the content before removing entities.");

        if (_contentType1.ContainsKey(id))
        {
            if (_contentType1.Remove(id))
            {
                Dirty = true;
            }
        }
        if (_contentType2.ContainsKey(id))
        {
            if (_contentType2.Remove(id))
            {
                Dirty = true;
            }
        }
    }
    
}