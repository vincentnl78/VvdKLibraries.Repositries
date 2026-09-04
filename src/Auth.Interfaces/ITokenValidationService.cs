using Auth.Objects;

namespace Auth.Interfaces;

public interface ITokenValidationService
{
    Task<JwtTokenValidationResult> ValidateToken(string tokenstring);
}

public struct JwtTokenValidationResult
{
    public string? UserId { get; init; }
    public string? TokenId { get; init; }
    public JwtTokenResult.AuthenticationErrorCodes Status { get; init; }
}