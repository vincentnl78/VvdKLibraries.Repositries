using System.Security.Claims;
using Auth.Interfaces;
using Auth.Interfaces.Storage;
using Microsoft.AspNetCore.Identity;

namespace Auth.Repositories;

// IUserRoleStore
// IUserClaimStore
// IUserPasswordStore
// IUserSecurityStampStore
// IUserEmailStore
// IUserPhoneNumberStore
// IQueryableUserStore
// IUserLoginStore
// IUserTwoFactorStore
// IUserLockoutStore

// Methods used

// FindByIdAsync
// FindByEmailAsync
// Get All
// CheckPasswordAsync
// GetClaimsAsync
// DeleteAsync
// ConfirmEmailAsync
// GenerateEmailConfirmationTokenAsync
// CreateAsync

public class CustomUserStore(
    IUsersGeneralTableRepositry usersGeneralTableRepositry,
    IUserClaimsGeneralTableRepositry claimsGeneralTableRepositry) :
    IUserPasswordStore<IdentityUser>,
    IUserEmailStore<IdentityUser>,
    IQueryableUserStore<IdentityUser>,
    IUserClaimStore<IdentityUser>
{
    public IQueryable<IdentityUser> Users
    {
        get
        {
            var all = usersGeneralTableRepositry.GetAll(CancellationToken.None);
            return all.AsQueryable();
        }
    }

    public Task<string?> GetEmailAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Email);
    }

    public Task SetEmailAsync(IdentityUser user, string? email, CancellationToken cancellationToken)
    {
        user.Email = email ?? throw new ArgumentNullException(nameof(email));
        return Task.CompletedTask;
    }

    public Task<bool> GetEmailConfirmedAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.EmailConfirmed);
    }

    public Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken cancellationToken)
    {
        user.EmailConfirmed = confirmed;
        return Task.CompletedTask;
    }

    public Task<string?> GetNormalizedEmailAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.NormalizedEmail);
    }

    public void Dispose()
    {
    }

    public Task SetUserNameAsync(IdentityUser user, string? userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;
        return Task.CompletedTask;
    }

    public Task<string?> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.NormalizedUserName);
    }

    public async Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        await usersGeneralTableRepositry.DeleteAsync(user);
        return IdentityResult.Success;
    }

    public Task<string?> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash);
    }

    public Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash != null);
    }

    #region Updates

    public Task SetPasswordHashAsync(IdentityUser user, string? passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;
        return Task.CompletedTask;
    }

    public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Id);
    }

    public Task<string?> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserName);
    }

    public async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        var result = await usersGeneralTableRepositry.AddAsync(user);
        return result ? IdentityResult.Success : IdentityResult.Failed();
    }

    public Task SetNormalizedUserNameAsync(IdentityUser user, string? normalizedName,
        CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    public async Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        bool result = await usersGeneralTableRepositry.UpdateAsync(user);
        return result ? IdentityResult.Success : IdentityResult.Failed();
    }

    #endregion

    #region Fetches

    public async Task<IdentityUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        return await usersGeneralTableRepositry.FetchByNameAsync(normalizedUserName, cancellationToken);
    }

    public async Task<IdentityUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await usersGeneralTableRepositry.FetchByIdAsync(userId, cancellationToken);
    }

    public async Task<IdentityUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        return await usersGeneralTableRepositry.FetchByEmailAsync(normalizedEmail, cancellationToken);
    }

    public Task SetNormalizedEmailAsync(IdentityUser user, string? normalizedEmail, CancellationToken cancellationToken)
    {
        user.NormalizedEmail = normalizedEmail ?? throw new ArgumentNullException(nameof(normalizedEmail));
        return Task.CompletedTask;
    }

    #endregion

    #region Claims

    public async Task<IList<Claim>> GetClaimsAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return await claimsGeneralTableRepositry.GetClaimsAsync(user.Id);
    }

    public async Task AddClaimsAsync(IdentityUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        await claimsGeneralTableRepositry.AddAsync(user.Id, claims, cancellationToken);
    }

    public async Task ReplaceClaimAsync(IdentityUser user, Claim claim, Claim newClaim,
        CancellationToken cancellationToken)
    {
        await claimsGeneralTableRepositry.DeleteAsync(user.Id, new List<Claim> { claim }, cancellationToken);
        await claimsGeneralTableRepositry.AddAsync(user.Id, new List<Claim> { newClaim }, cancellationToken);
    }

    public async Task RemoveClaimsAsync(IdentityUser user, IEnumerable<Claim> claims,
        CancellationToken cancellationToken)
    {
        await claimsGeneralTableRepositry.DeleteAsync(user.Id, claims, cancellationToken);
    }

    public async Task<IList<IdentityUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
    {
        var users = await claimsGeneralTableRepositry.GetUsersForClaimAsync(claim, cancellationToken);
        List<IdentityUser> userEntities = [];
        foreach (var te in users)
        {
            var user = await FindByIdAsync(te, cancellationToken);
            if (user != null)
            {
                userEntities.Add(user);
            }
        }

        return userEntities;
    }

    #endregion
}