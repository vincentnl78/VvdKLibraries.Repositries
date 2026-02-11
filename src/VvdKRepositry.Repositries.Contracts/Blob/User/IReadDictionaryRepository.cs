namespace VvdKRepositry.Repositries.Contracts.Blob.User;

public interface IReadDictionaryRepository<TKey,T> : IReadRepository<TKey,T>,IBaseBacking
    where T : IId<TKey>
{
    IReadOnlyDictionary<TKey,T> Dictionary { get; }
}