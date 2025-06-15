namespace VvdKRepositry.Repositries.Contracts.Blob.User;

public interface IWriteRepository<in TKey,T>:IBaseBacking
    where TKey : struct
    where T :IId<TKey>
{
    T Add(T entity);
    void Update(T entity);
    void Update(IEnumerable<T> entity);
    void Remove(TKey id);
}