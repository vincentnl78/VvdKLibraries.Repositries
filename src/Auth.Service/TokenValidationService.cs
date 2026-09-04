using System.Text;
using Auth.Interfaces;
using Auth.Objects;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Auth.Service;

public class TokenValidationService(
    JwtAuthenticationSettings jwtAuthenticationSettings,
    TimeProvider timeProvider
) : ITokenValidationService
{
    private SymmetricSecurityKey SymmetricSecurityKey =>
        field ??= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthenticationSettings.Key));

    private TokenValidationParameters TokenValidationParameters => field ??= new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtAuthenticationSettings.SiteUrl,

        ValidateAudience = true,
        ValidAudience = jwtAuthenticationSettings.SiteUrl,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = SymmetricSecurityKey,

        ValidateLifetime = true, // Ensure the token hasn't expired
        ClockSkew = TimeSpan.Zero, // Optional: No clock skew tolerance

        LifetimeValidator = // Custom lifetime logic (ignore expiration or simulate time)
            (notBefore, expires, _, _) =>
            {
                var now = timeProvider.GetUtcNow(); // your "current" time
                // Example: ignore expiration
                return (!notBefore.HasValue || notBefore <= now)
                       && (!expires.HasValue || expires > now);
            },
        IncludeTokenOnFailedValidation = true
    };

    public async Task<JwtTokenValidationResult> ValidateToken(string tokenstring)
    {
        var tokenHandler = new JsonWebTokenHandler();
        var result = await tokenHandler.ValidateTokenAsync(tokenstring, TokenValidationParameters);

        if (result.IsValid)
            return new JwtTokenValidationResult()
            {
                Status = JwtTokenResult.AuthenticationErrorCodes.Ok,
                UserId = result.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Sub)?.Value,
                TokenId = result.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Jti)?.Value
            };

        var token = result.TokenOnFailedValidation as JsonWebToken;
        if (token == null)
        {
            return new JwtTokenValidationResult()
            {
                Status = JwtTokenResult.AuthenticationErrorCodes.InvalidRequest,
            };
        }

        switch (result.Exception)
        {
            case SecurityTokenInvalidLifetimeException:
                return new JwtTokenValidationResult()
                {
                    Status = JwtTokenResult.AuthenticationErrorCodes.Expired,
                    UserId = token.Subject,
                    TokenId = token.Id
                };
            case SecurityTokenSignatureKeyNotFoundException:
                return new JwtTokenValidationResult()
                {
                    Status = JwtTokenResult.AuthenticationErrorCodes.InvalidRequest,
                    UserId = token.Subject,
                    TokenId = token.Id
                };
            default:
                Log.Error("Token validation failed: {Error} for {Token}", result.Exception?.Message, tokenstring);
                return new JwtTokenValidationResult()
                {
                    Status = JwtTokenResult.AuthenticationErrorCodes.ServerError,
                    UserId = token.Subject,
                    TokenId = token.Id
                };
        }
    }
}