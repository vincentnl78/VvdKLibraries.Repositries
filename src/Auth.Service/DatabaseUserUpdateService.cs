using System.Text;
using Auth.Interfaces;
using Auth.Objects;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Auth.Service;

public sealed class UserUpdateService(UserManager<IdentityUser> userManager) : IDatabaseUserUpdateService
{
    public async Task<UserResults> DeleteAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user != null
            ? Convert(await userManager.DeleteAsync(user))
            : UserResults.NotFound;
    }

    public async Task<UserResults> ConfirmEmailAsync(string id,
        string emailConfirmationToken)
    {
        var user = await userManager.FindByIdAsync(id);
        var token = Base64Decode(emailConfirmationToken);
        if (user != null)
            return Convert(await userManager.ConfirmEmailAsync(user, token));
        return UserResults.NotFound;
    }

    public async Task<(UserResults result, string token)> GenerateEmailConfirmationTokenAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return (UserResults.NotFound, string.Empty);
        }

        if (user.EmailConfirmed)
            return (UserResults.AlreadyConfirmed, string.Empty);

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        return (UserResults.Ok, Base64Encode(token));
    }


    public async Task<UserResults> CreateAsync(string email, string password, ApiAreas apiArea)
    {
        var user = new IdentityUser
        {
            Id = UserConversions.GenerateId(apiArea),
            Email = email,
            UserName = email
        };
        return Convert(await userManager.CreateAsync(user, password));
    }

    public static string Base64Encode(string text)
    {
        var textBytes = Encoding.UTF8.GetBytes(text);
        return System.Convert.ToBase64String(textBytes);
    }

    private static string Base64Decode(string base64)
    {
        var base64Bytes = System.Convert.FromBase64String(base64);
        return Encoding.UTF8.GetString(base64Bytes);
    }

    private UserResults Convert(IdentityResult ir)
    {
        if (ir.Succeeded) return UserResults.Ok;
        foreach (var identityError in ir.Errors)
            switch (identityError.Code)
            {
                case "500":
                    Log.Error("Unknown failure: from Identity:{IdentityErrorDescription}",
                        identityError.Description);
                    return UserResults.SystemCrash;
                case "InvalidToken":
                    Log.Error("{Reason} - confirming email", identityError.Description);
                    return UserResults.InvalidToken;
                case "InvalidUserName":
                    Log.Error("{Reason} - creating user", identityError.Description);
                    return UserResults.InvalidUsername;
                case "PasswordTooShort":
                    Log.Error("{Reason} - creating user", identityError.Description);
                    return UserResults.InvalidPasswordOrUnkownUser;
            }

        throw new ArgumentException("Unknown Identity result error code");
    }
}