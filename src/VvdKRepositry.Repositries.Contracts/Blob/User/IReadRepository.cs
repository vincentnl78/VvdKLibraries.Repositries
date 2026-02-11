using System.Diagnostics.CodeAnalysis;

namespace VvdKRepositry.Repositries.Contracts.Blob.User;

public interface IReadRepository<in TKey,T>:IBaseBacking
    where T : IId<TKey> 
{
    IEnumerable<T> All { get; }
    bool TryGetValue(TKey id, [NotNullWhen(true)] out T value);
}