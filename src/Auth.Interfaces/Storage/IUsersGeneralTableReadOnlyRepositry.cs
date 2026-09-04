using Microsoft.AspNetCore.Identity;

namespace Auth.Interfaces.Storage;

public interface IUsersGeneralTableReadOnlyRepositry
{
    Task<IdentityUser?> FetchByNameAsync(string normalizedUserName, CancellationToken cancellationToken);
    Task<IdentityUser?> FetchByIdAsync(string userId, CancellationToken cancellationToken);
    List<IdentityUser> GetAll(CancellationToken cancellationToken);
    Task<List<IdentityUser>> GetAllAsync(CancellationToken cancellationToken);
    Task<IdentityUser?> FetchByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);
}