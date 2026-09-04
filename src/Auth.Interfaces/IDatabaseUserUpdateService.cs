using Auth.Objects;

namespace Auth.Interfaces;

public interface IDatabaseUserUpdateService
{
    Task<UserResults> DeleteAsync(string id);
    Task<UserResults> ConfirmEmailAsync(string id, string emailConfirmationToken);
    Task<(UserResults result, string token)> GenerateEmailConfirmationTokenAsync(string id);
    Task<UserResults> CreateAsync(string email, string password, ApiAreas apiArea);
}