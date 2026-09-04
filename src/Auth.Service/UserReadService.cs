using Auth.Interfaces;
using Auth.Interfaces.Storage;
using Auth.Objects;

namespace Auth.Service;

public class UserReadService(
    IUsersGeneralTableReadOnlyRepositry usersGeneralTableReadOnlyRepositry)
    : IUserReadService
{
    public virtual async Task<SlimUser?> FindByIdAsync(string id)
    {
        var user = await usersGeneralTableReadOnlyRepositry.FetchByIdAsync(id, CancellationToken.None);
        return user?.ToSlimUser();
    }

    public virtual async Task<SlimUser?> FindByEmailAsync(string email)
    {
        var user = await usersGeneralTableReadOnlyRepositry.FetchByEmailAsync(email, CancellationToken.None);
        return user?.ToSlimUser();
    }

    public virtual async Task<List<SlimUser>> GetAllAsync()
    {
        var allUsers = await usersGeneralTableReadOnlyRepositry.GetAllAsync(CancellationToken.None);
        return allUsers.Select(UserConversionExtensions.ToSlimUser).ToList();
    }
    
    public virtual List<SlimUser> GetAll()
    {
        return usersGeneralTableReadOnlyRepositry.GetAll(CancellationToken.None).Select(UserConversionExtensions.ToSlimUser).ToList();
    }
}