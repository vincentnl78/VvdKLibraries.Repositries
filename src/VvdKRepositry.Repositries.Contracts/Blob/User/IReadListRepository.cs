using System.Linq.Expressions;

namespace VvdKRepositry.Repositries.Contracts.Blob.User;

public interface IReadListRepository<out TEntity> : ILoadable
    where TEntity : class, IIntId
{
    IEnumerable<TEntity> All { get; }
    TEntity? GetById(int id);
}