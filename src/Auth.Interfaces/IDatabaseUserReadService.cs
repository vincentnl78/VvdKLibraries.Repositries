using Auth.Objects;

namespace Auth.Interfaces;

public interface IUserValidationService
{
    Task<UserInfo> GetValidatedUserInfo(string email, string password);
    Task<UserInfo> GetValidatedUserInfo(string userid);

    public class UserInfo
    {
        public bool IsPasswordValid { get; init; }

        public SlimUser? User { get; init; }
        //public List<Claim> ProfileClaims { get; init; } = [];
    }
}

public interface IUserReadService
{
    Task<SlimUser?> FindByIdAsync(string id);
    Task<SlimUser?> FindByEmailAsync(string email);
    List<SlimUser> GetAll();
    Task<List<SlimUser>> GetAllAsync();
}

public interface IDatabaseUserExternalIdentityProviderService
{
    Task<(UserResults, string?)> FetchEmailByExternalAuthorizationLocal(string? code,
        IdentityAuthenticationProviders? provider);
}