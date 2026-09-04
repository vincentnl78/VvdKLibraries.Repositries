using Auth.Interfaces;
using Auth.Objects;
using Serilog;

namespace Auth.Service;

public class TokenGenerationService(
    IDatabaseUserTokenGenerationService databaseUserTokenGenerationService
) : ITokenGenerationSerivce
{
    public async Task<JwtTokenResult> GenerateJwtTokenAsync(string? loginEmail, string? loginPassword)
    {
        if (loginEmail == null || loginPassword == null)
        {
            return new JwtTokenResult(JwtTokenResult.AuthenticationErrorCodes.InvalidRequest,
                "Login email or password is null")
            {
                UserId = string.Empty
            };
        }

        var result = await databaseUserTokenGenerationService.GenerateNewJwtToken(loginEmail, loginPassword);
        if (string.IsNullOrEmpty(result.UserId))
        {
            Log.Warning("GenerateJwtTokenAsync: UserId is not found for email {Email}", loginEmail);
        }
        return result;
    }


    public async Task<JwtTokenResult> RefreshJwtTokenAsync(string? refreshloginRefreshtoken)
    {
        if (refreshloginRefreshtoken == null)
        {
            return new JwtTokenResult(JwtTokenResult.AuthenticationErrorCodes.InvalidRequest,
                "Token or refresh token is null")
            {
                UserId = string.Empty
            };
        }

        var result = await databaseUserTokenGenerationService.RefreshJwtToken(refreshloginRefreshtoken);
        if (string.IsNullOrEmpty(result.UserId))
        {
            Log.Warning("RefreshJwtTokenAsync: UserId is not found for token {Token}", refreshloginRefreshtoken);
        }
        return result;
    }
}