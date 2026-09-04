using Auth.Objects;

namespace Auth.Interfaces;

public interface IDatabaseUserTokenGenerationService
{
    //todo refactor this to userid
    Task<JwtTokenResult> GenerateNewJwtToken(string email, string loginPassword);
    Task<JwtTokenResult> RefreshJwtToken(string refreshtoken);
    Task<List<RefreshToken>> GetRefreshTokensByUserId(string userid);
    public Task CleanUpRefreshTokens();
}