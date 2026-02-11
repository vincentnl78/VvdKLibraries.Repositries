namespace VvdKRepositry.Repositries.Contracts.Blob.User;

public interface ILockableByLease
{
    public Task<bool> AcquireLeaseAsync(TimeSpan timespan, CancellationToken cancellationToken = default);
    public Task<bool> ReleaseLease(bool force);
    Task<DateTimeOffset?> GetStartOfCurrentLeaseAsync();
}