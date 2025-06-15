namespace VvdKRepositry.Repositries.Contracts.Blob.User;

public interface IWriteRepository<TEntity> : IDirtyable where TEntity : class, IIntId
{
    TEntity Add(TEntity entity);
    void Update(TEntity entity);
    void Remove(int id);
}