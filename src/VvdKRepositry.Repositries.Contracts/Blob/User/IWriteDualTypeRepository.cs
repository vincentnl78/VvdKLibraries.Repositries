namespace VvdKRepositry.Repositries.Contracts.Blob.User;

public interface IWriteDualTypeRepository<in TKey,T1,T2>:IBaseBacking
    where TKey : struct
    where T1 :IId<TKey>
    where T2 :IId<TKey>
{
    T1 Add(T1 entity);
    T2 Add(T2 entity);
    void Update(T2 entity);
    void Update(T1 entity);
    void Remove(TKey id);
}