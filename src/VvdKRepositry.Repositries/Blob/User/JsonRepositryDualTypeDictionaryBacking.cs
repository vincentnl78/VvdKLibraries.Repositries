using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using VvdKRepositry.Repositries.Contracts.Blob.User;

namespace VvdKRepositry.Repositries.Blob.User;

public abstract class JsonRepositryDualTypeDictionaryBacking<T,T1,T2>(
    IUserBlobPersistence persistence,
    JsonSerializerOptions jsonSerializerOptions)
    : JsonRepositryBaseBacking<Dictionary<int,T>>(persistence, jsonSerializerOptions),
        IReadDualTypeDictionaryRepository<T,T1,T2>,
        IWriteDualTypeRepository<int,T1,T2>
    where T : EntityWithId<int>
    where T1:T
    where T2:T
{

    private Dictionary<int,T1>? _contentType1;
    private Dictionary<int,T2>? _contentType2;

    public IReadOnlyDictionary<int, T1> T1All => _contentType1 ?? [];
    public IReadOnlyDictionary<int, T2> T2All => _contentType2 ?? [];
    public IEnumerable<T> All
    {
        get
        {
            if (_contentType1?.Values is { } first && _contentType2?.Values is { } second)
                return first.Concat<T>(second);
            if (_contentType1?.Values is { } onlyFirst)
                return onlyFirst;
            if (_contentType2?.Values is { } onlySecond)
                return onlySecond;
            return [];
        }
    }
      
    public bool TryGet(int id,[NotNullWhen(true)] out T value)
    {
        if (_contentType1 != null && _contentType1.TryGetValue(id, out var t1))
        {
            value = t1;
            return true;
        }
        if (_contentType2 != null && _contentType2.TryGetValue(id, out var t2))
        {
            value = t2;
            return true;
        }
        value = null!;
        return false;
    }

    public IReadOnlyDictionary<int, T> Dictionary =>
        Content;
    
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

    [MemberNotNull("_contentType1", "_contentType2")]
    private TEntity UpdateId<TEntity>(TEntity entity)
        where TEntity : EntityWithId<int>
    {
        if(_contentType1 == null || _contentType2 == null)
            throw new InvalidOperationException("Content dictionaries are not initialized. Ensure you have loaded the content before updating entities.");
        return entity.Id == 0
            ? entity with
            {
                Id = Math.Max(
                    _contentType1.Count != 0 ? _contentType1.Keys.Max() : 0,
                    _contentType2.Count != 0 ? _contentType2.Keys.Max() : 0
                )+1
            }
            : entity;
    }

    public T1 Add(T1 entity)
    {
        entity =UpdateId(entity);
        _contentType1[entity.Id] = UpdateId(entity);
        Dirty = true;
        return entity;
    }

    public T2 Add(T2 entity)
    {
        entity =UpdateId(entity);
        _contentType2[entity.Id] = UpdateId(entity);
        Dirty = true;
        return entity;
    }

    public virtual void Update(T1 entity)
    {
        if (_contentType1 == null)
            throw new InvalidOperationException("Content dictionaries are not initialized. Ensure you have loaded the content before updating entities.");
        _contentType1[entity.Id] = entity;
        Dirty = true;
    }

    public void Update(T2 entity)
    {
        if (_contentType2 == null)
            throw new InvalidOperationException("Content dictionaries are not initialized. Ensure you have loaded the content before updating entities.");
        _contentType2[entity.Id] = entity;
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