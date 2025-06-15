namespace VvdKRepositry.Repositries.Contracts.Blob.User;

public interface IReadDictionaryRepository<TKey,T> : IReadRepository<TKey,T>,IBaseBacking
    where T : IId<TKey>
    where TKey :struct
{
    IReadOnlyDictionary<TKey,T> Dictionary { get; }
}