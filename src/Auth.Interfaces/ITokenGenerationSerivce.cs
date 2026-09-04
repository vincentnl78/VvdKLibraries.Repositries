using Auth.Objects;

namespace Auth.Interfaces;

public interface ITokenGenerationSerivce
{
    Task<JwtTokenResult> GenerateJwtTokenAsync(string? loginEmail, string? loginPassword);
    Task<JwtTokenResult> RefreshJwtTokenAsync(string? refreshToken);
}