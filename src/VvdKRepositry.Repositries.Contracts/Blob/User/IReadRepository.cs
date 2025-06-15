namespace VvdKRepositry.Repositries.Contracts.Blob.User;

public interface IReadRepository<out TEntity> : ILoadable
    where TEntity : class, IIntId
{
    IEnumerable<TEntity> All { get; }
    TEntity? GetById(int id);
}