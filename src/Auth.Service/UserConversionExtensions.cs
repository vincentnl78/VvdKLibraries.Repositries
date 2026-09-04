using Auth.Objects;
using Microsoft.AspNetCore.Identity;

namespace Auth.Service;

public static class UserConversionExtensions
{
    public static SlimUser ToSlimUser(this IdentityUser user)
    {
        return new SlimUser
        {
            Id = user.Id ?? throw new Exception("Id is null"),
            Email = user.Email ?? throw new Exception("Email is null"),
            EmailConfirmed = user.EmailConfirmed
        };
    }
}