using Microsoft.AspNetCore.Identity;

namespace Auth.Interfaces.Storage;

public interface IUsersGeneralTableRepositry:IUsersGeneralTableReadOnlyRepositry
{
    Task<bool> AddAsync(IdentityUser user);
    Task<bool> UpdateAsync(IdentityUser user);
    Task DeleteAsync(IdentityUser user);
}