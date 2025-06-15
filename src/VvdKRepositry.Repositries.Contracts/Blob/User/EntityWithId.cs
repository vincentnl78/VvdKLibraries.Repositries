namespace VvdKRepositry.Repositries.Contracts.Blob.User;

public abstract record EntityWithId<TKey> : IId<TKey> 
    where TKey : struct
{
    public TKey Id { get; init; }
}