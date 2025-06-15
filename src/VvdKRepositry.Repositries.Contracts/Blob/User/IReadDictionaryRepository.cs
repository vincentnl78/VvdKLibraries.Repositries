namespace VvdKRepositry.Repositries.Contracts.Blob.User;

public interface IReadDictionaryRepository<TEntity> : ILoadable, IReadRepository<TEntity>
    where TEntity : class, IIntId
{
    IReadOnlyDictionary<int,TEntity> Dictionary { get; }
}