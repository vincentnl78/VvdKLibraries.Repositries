namespace VvdKRepositry.Repositries.Contracts.Blob.User;

public abstract record Aggregate<TKey> : IGuidId where TKey:struct
{
    List<TKey> Ids { get; }= new List<TKey>();
    private DateTimeOffset StartDate { get; init; }
    DateTimeOffset EndDate { get;init; }
    public Guid Id { get; init; }
}