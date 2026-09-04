using System.Security.Claims;

namespace Auth.Interfaces;

public interface IUserClaimsGeneralTableRepositry
{
    Task<IList<Claim>> GetClaimsAsync(string userId);
    Task AddAsync(string userId, IEnumerable<Claim> claims, CancellationToken cancellationToken);
    Task DeleteAsync(string userId, IEnumerable<Claim> claims, CancellationToken cancellationToken);
    Task<IList<string>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken);
}