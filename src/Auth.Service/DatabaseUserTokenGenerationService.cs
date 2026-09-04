using System.Security.Claims;
using System.Text;
using Auth.Interfaces;
using Auth.Objects;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Service;

public class DatabaseUserTokenGenerationService(
    IUserValidationService userValidationService,
    ITokenValidationService tokenValidationService,
    IRefreshTokensTableRepositry refreshTokensTableRepositry,
    RegionalBackendUrlConnectionStrings apiUrls,
    JwtAuthenticationSettings jwtAuthenticationSettings,
    TimeProvider timeProvider
)
    : IDatabaseUserTokenGenerationService
{
    private SymmetricSecurityKey SymmetricSecurityKey =>
        field ??= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthenticationSettings.Key));


    private SigningCredentials Credentials => field ??= new SigningCredentials(
        SymmetricSecurityKey,
        SecurityAlgorithms.HmacSha256);

    public async Task<JwtTokenResult> RefreshJwtToken(string refreshtoken)
    {
        var validationResult = await tokenValidationService.ValidateToken(refreshtoken);
        if (validationResult.UserId == null)
        {
            return new JwtTokenResult(JwtTokenResult.AuthenticationErrorCodes.InvalidLogin, "No User Id in Token")
            {
                UserId = string.Empty
            };
        }

        if (validationResult.TokenId == null)
        {
            return new JwtTokenResult(JwtTokenResult.AuthenticationErrorCodes.InvalidLogin,
                "Not a valid refresh token")
            {
                UserId = validationResult.UserId
            };
        }

        switch (validationResult.Status)
        {
            case JwtTokenResult.AuthenticationErrorCodes.Ok:
                break;
            case JwtTokenResult.AuthenticationErrorCodes.InvalidLogin:
                return new JwtTokenResult(JwtTokenResult.AuthenticationErrorCodes.InvalidLogin,
                    "Invalid Login")
                {
                    UserId = validationResult.UserId
                };
            case JwtTokenResult.AuthenticationErrorCodes.Expired:
                return new JwtTokenResult(JwtTokenResult.AuthenticationErrorCodes.Expired, "Token Expired")
                {
                    UserId = validationResult.UserId
                };
            case JwtTokenResult.AuthenticationErrorCodes.ServerError:
                return new JwtTokenResult(JwtTokenResult.AuthenticationErrorCodes.Expired, "Token Expired")
                {
                    UserId = validationResult.UserId
                };
            case JwtTokenResult.AuthenticationErrorCodes.InvalidRequest:
                return new JwtTokenResult(JwtTokenResult.AuthenticationErrorCodes.InvalidRequest,
                    "Invalid Request")
                {
                    UserId = validationResult.UserId
                };

            default:
                throw new ArgumentOutOfRangeException();
        }

        var userinfo = await userValidationService.GetValidatedUserInfo(validationResult.UserId);
        if (userinfo.User == null)
        {
            return new JwtTokenResult(JwtTokenResult.AuthenticationErrorCodes.InvalidLogin, "User not found")
            {
                UserId = validationResult.UserId
            };
        }

        var tokenFromStorage = await refreshTokensTableRepositry.FetchAsync(
            userinfo.User.Id,
            validationResult.TokenId,
            CancellationToken.None);


        if (tokenFromStorage is null)
        {
            return new JwtTokenResult(JwtTokenResult.AuthenticationErrorCodes.Expired,
                "Refresh Token already used")
            {
                UserId = userinfo.User.Id
            };
        }

        await refreshTokensTableRepositry.DeleteAsync(tokenFromStorage.UserId, tokenFromStorage.Id);

        return await GenerateTokenAsync(userinfo.User, []);
    }

    public async Task<JwtTokenResult> GenerateNewJwtToken(string email, string loginPassword)
    {
        var userinfo = await userValidationService.GetValidatedUserInfo(email, loginPassword);
        if (userinfo.User == null)
        {
            return new JwtTokenResult(JwtTokenResult.AuthenticationErrorCodes.InvalidLogin,
                "Invalid Credentials")
            {
                UserId = string.Empty
            };
        }

        if (!userinfo.IsPasswordValid)
        {
            return new JwtTokenResult(JwtTokenResult.AuthenticationErrorCodes.InvalidLogin,
                "Invalid Credentials")
            {
                UserId = userinfo.User.Id
            };
        }

        if (!userinfo.User.EmailConfirmed)
            return new JwtTokenResult(JwtTokenResult.AuthenticationErrorCodes.InvalidLogin,
                "Email not confirmed")
            {
                UserId = userinfo.User.Id
            };

        return await GenerateTokenAsync(userinfo.User, []);
    }

    public async Task<List<RefreshToken>> GetRefreshTokensByUserId(string userid)
    {
        return await refreshTokensTableRepositry.FetchAsync(userid, CancellationToken.None);
    }


    public async Task CleanUpRefreshTokens()
    {
        await refreshTokensTableRepositry.CleanupAsync(CancellationToken.None);
    }


    private async Task<JwtTokenResult> GenerateTokenAsync(SlimUser user, List<Claim> profileClaims)
    {
        var now = timeProvider.GetUtcNow().DateTime;
        var tokenExpiration = now.AddMinutes(jwtAuthenticationSettings.TokenExpirationMinutes);
        var refreshTokenExpiration = now.AddMinutes(jwtAuthenticationSettings.RefreshTokenExpirationMinutes);

        var claims = GetIdAndUrlTokenClaims(user);
        claims = claims.Union(profileClaims).ToList();

        var token = GetJwtSecurityToken(
            claims,
            now,
            tokenExpiration);

        var tokenId = Guid.NewGuid().ToString();
        var refreshToken = GenerateRefreshToken(user.Id, tokenId, now, refreshTokenExpiration);
        await refreshTokensTableRepositry.AddAsync(new RefreshToken
        {
            Id = tokenId,
            UserId = user.Id,
            Token = refreshToken,
            Expiration = timeProvider.GetUtcNow().AddMinutes(jwtAuthenticationSettings.RefreshTokenExpirationMinutes)
        }, CancellationToken.None);
        return new JwtTokenResult(token, refreshToken, tokenExpiration)
        {
            UserId = user.Id
        };
    }

    private string GenerateRefreshToken(string userid, string tokenid, DateTime now, DateTime expiration)
    {
        var refreshDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(JwtRegisteredClaimNames.Jti, tokenid),
                new Claim(JwtRegisteredClaimNames.Sub, userid)
            ]),
            Issuer = jwtAuthenticationSettings.SiteUrl,
            Audience = jwtAuthenticationSettings.SiteUrl,
            NotBefore = now,
            Expires = expiration,
            SigningCredentials = Credentials
        };
        return new JsonWebTokenHandler().CreateToken(refreshDescriptor);
    }

    private string GetJwtSecurityToken(
        IList<Claim> claims,
        DateTime now,
        DateTime expiration
    )
    {
        var accessDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiration,
            Issuer = jwtAuthenticationSettings.SiteUrl,
            Audience = jwtAuthenticationSettings.SiteUrl,
            NotBefore = now,
            SigningCredentials = Credentials
        };
        var token = new JsonWebTokenHandler().CreateToken(accessDescriptor);
        return token;
    }

    private IList<Claim> GetIdAndUrlTokenClaims(SlimUser user)
    {
        var area = UserConversions.GetApiAreaFromId(user.Id);
        var regional = apiUrls[area];

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtToken.ApiBackEndUrlClaimIdentifier, regional)
        ];
        return claims;
    }
}