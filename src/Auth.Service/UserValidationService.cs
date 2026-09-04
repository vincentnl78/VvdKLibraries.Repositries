using Auth.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Auth.Service;

public class UserValidationService(
    UserManager<IdentityUser> userManager
): IUserValidationService
{
    public async Task<IUserValidationService.UserInfo> GetValidatedUserInfo(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return new IUserValidationService.UserInfo
            {
                User = null,
                IsPasswordValid = false,
            };
        }

        var isValid = await userManager.CheckPasswordAsync(user, password);
        if (!isValid)
            return new IUserValidationService.UserInfo()
            {
                User = user.ToSlimUser(),
                IsPasswordValid = false
            };

        return await GetValidatedUserInfo(user);
    }

    public async Task<IUserValidationService.UserInfo> GetValidatedUserInfo(string userid)
    {
        var user = await userManager.FindByIdAsync(userid);
        if (user == null)
        {
            return new IUserValidationService.UserInfo
            {
                User = null,
                IsPasswordValid = false,
            };
        }

        return await GetValidatedUserInfo(user);
    }

    private async Task<IUserValidationService.UserInfo> GetValidatedUserInfo(IdentityUser user)
    {
        //var claims = await userManager.GetClaimsAsync(user);
        return new IUserValidationService.UserInfo()
        {
            User = user.ToSlimUser(),
            IsPasswordValid = true,
            //ProfileClaims = claims.ToList()
        };
    }
    
}