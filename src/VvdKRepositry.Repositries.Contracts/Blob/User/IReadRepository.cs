using System.Diagnostics.CodeAnalysis;

namespace VvdKRepositry.Repositries.Contracts.Blob.User;

public interface IReadRepository<in TKey,T>:IBaseBacking
    where T : IId<TKey> 
    where TKey : struct
{
    IEnumerable<T> All { get; }
    bool TryGet(TKey id, [NotNullWhen(true)] out T value);
}