using System.Linq.Expressions;

namespace VvdKRepositry.Repositries.Contracts.Blob.User;

public interface IReadDualTypeDictionaryRepository<TEntity,TEntity1,TEntity2> : ILoadable
    where TEntity : class, IIntId
    where TEntity1 : TEntity
    where TEntity2 : TEntity
{
    IReadOnlyDictionary<int,TEntity1> T1All { get; }
    IReadOnlyDictionary<int,TEntity2> T2All { get; }
}