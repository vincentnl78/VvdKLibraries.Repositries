using System.Linq.Expressions;

namespace VvdKRepositry.Repositries.Contracts.Blob.User;

public interface IReadDictionaryRepository<TEntity> : ILoadable
    where TEntity : class, IIntId
{
    IReadOnlyDictionary<int,TEntity> All { get; }
    TEntity? GetById(int id);
}