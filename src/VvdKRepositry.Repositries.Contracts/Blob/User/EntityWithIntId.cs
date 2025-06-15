namespace VvdKRepositry.Repositries.Contracts.Blob.User;

public abstract record EntityWithIntId : IIntId
{
    public int Id { get; init; }
}