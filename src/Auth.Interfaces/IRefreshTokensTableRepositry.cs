using Auth.Objects;

namespace Auth.Interfaces;

public interface IRefreshTokensTableRepositry
{
    Task<List<RefreshToken>> FetchAsync(string userid, CancellationToken cancellationToken);
    Task<RefreshToken?> FetchAsync(string userid, string tokenid, CancellationToken cancellationToken);
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task CleanupAsync(CancellationToken cancellationToken);
    Task DeleteAsync(string userId, string id);
}