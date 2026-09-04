using Auth.Interfaces.Storage;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Identity;
using VvdKRepositry.Repositries.Table.Base;

namespace Auth.Repositories;

public class UsersGeneralTableRepositry(IIdentityTablePersistence persistence)
    : BaseTableRepositryWithCreationNotifiers(persistence), IUsersGeneralTableRepositry
{
    protected override string TableName => "Users";
    private static string UserByIdPartition => "UsersById";
    private static string UserByEmailPartition => "UsersByEmail";

    public async Task<bool> AddAsync(IdentityUser user)
    {
        var userByEmail = Convert(user);
        userByEmail.PartitionKey = UserByEmailPartition;
        userByEmail.RowKey = user.NormalizedEmail;
        var userById = Convert(user);

        var success = await SubmitChangesAsync([userByEmail, userById], null, null);
        return success;
    }

    public async Task<bool> UpdateAsync(IdentityUser user)
    {
        var userByEmail = Convert(user);
        userByEmail.PartitionKey = UserByEmailPartition;
        userByEmail.RowKey = user.NormalizedEmail;
        var userById = Convert(user);


        return await SubmitChangesAsync(null, [userByEmail, userById], null);
    }

    public async Task<IdentityUser?> FetchByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        var pages = await FetchByPartitionAndPropertyAsync(UserByIdPartition, nameof(IdentityUser.NormalizedUserName),
            normalizedUserName, 1, cancellationToken);
        var entity = pages.FirstOrDefault();
        return entity == null ? null : Convert(entity);
    }

    public async Task<IdentityUser?> FetchByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var entity = await FetchEntityAsync(UserByIdPartition, userId, cancellationToken);
        return entity == null ? null : Convert(entity);
    }

    public async Task DeleteAsync(IdentityUser user)
    {
        await DeleteAsync(UserByIdPartition, user.Id);
        if (user.NormalizedEmail != null)
            await DeleteAsync(UserByEmailPartition, user.NormalizedEmail);
    }

    public List<IdentityUser> GetAll(CancellationToken cancellationToken)
    {
        var pages = FetchPartition(UserByIdPartition, int.MaxValue, cancellationToken);
        return pages.Select(Convert).ToList();
    }

    public async Task<List<IdentityUser>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await FetchPartitionAsync(UserByIdPartition);
        return users.Select(Convert).ToList();
    }

    public async Task<IdentityUser?> FetchByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        var entity = await FetchEntityAsync(UserByEmailPartition, normalizedEmail.ToUpper(), cancellationToken);
        return entity == null ? null : Convert(entity);
    }

    private TableEntity Convert(IdentityUser user)
    {
        return new TableEntity(UserByIdPartition, user.Id)
        {
            ["Email"] = user.Email,
            ["NormalizedEmail"] = user.NormalizedEmail,
            ["PasswordHash"] = user.PasswordHash,
            ["SecurityStamp"] = user.SecurityStamp,
            ["ConcurrencyStamp"] = user.ConcurrencyStamp,
            ["PhoneNumber"] = user.PhoneNumber,
            ["PhoneNumberConfirmed"] = user.PhoneNumberConfirmed,
            ["TwoFactorEnabled"] = user.TwoFactorEnabled,
            ["LockoutEnd"] = user.LockoutEnd,
            ["LockoutEnabled"] = user.LockoutEnabled,
            ["AccessFailedCount"] = user.AccessFailedCount,
            ["EmailConfirmed"] = user.EmailConfirmed,
            ["NormalizedUserName"] = user.NormalizedUserName,
            ["UserName"] = user.UserName,
            ["Id"] = user.Id
        };
    }

    private IdentityUser Convert(TableEntity tableEntity)
    {
        /*var id = tableEntity["Id"].ToString();
        var userName = tableEntity["UserName"].ToString();
        var normalizedUserName = tableEntity["NormalizedUserName"].ToString();
        var email = tableEntity["Email"].ToString();
        var normalizedEmail = tableEntity["NormalizedEmail"].ToString();
        var emailConfirmed = bool.Parse(tableEntity["EmailConfirmed"].ToString() ?? string.Empty);
        var passwordHash = tableEntity["PasswordHash"].ToString();
        var securityStamp = tableEntity["SecurityStamp"].ToString();
        var concurrencyStamp = tableEntity["ConcurrencyStamp"].ToString();
        var phoneNumber = tableEntity["PhoneNumber"].ToString();
        var phoneNumberConfirmed = bool.Parse(tableEntity["PhoneNumberConfirmed"].ToString() ?? string.Empty);
        var twoFactorEnabled = bool.Parse(tableEntity["TwoFactorEnabled"].ToString() ?? string.Empty);
        //var lockoutEnd = tableEntity["LockoutEnd"].ToString();
        var lockoutEnabled = bool.Parse(tableEntity["LockoutEnabled"].ToString() ?? string.Empty);
        var accessFailedCount = int.Parse(tableEntity["AccessFailedCount"].ToString() ?? string.Empty);*/
        
        
        return new IdentityUser
        {
            Id = tableEntity["Id"].ToString() ?? string.Empty,
            UserName = tableEntity["UserName"].ToString(),
            NormalizedUserName = tableEntity["NormalizedUserName"].ToString(),
            Email = tableEntity["Email"].ToString() ?? string.Empty,
            NormalizedEmail = tableEntity["NormalizedEmail"].ToString() ?? string.Empty,
            EmailConfirmed = bool.Parse(tableEntity["EmailConfirmed"].ToString() ?? string.Empty),
            PasswordHash = tableEntity["PasswordHash"].ToString(),
            SecurityStamp = tableEntity["SecurityStamp"].ToString(),
            ConcurrencyStamp = tableEntity["ConcurrencyStamp"].ToString(),
            //PhoneNumber = tableEntity["PhoneNumber"].ToString(),
            PhoneNumberConfirmed = bool.Parse(tableEntity["PhoneNumberConfirmed"].ToString() ?? string.Empty),
            TwoFactorEnabled = bool.Parse(tableEntity["TwoFactorEnabled"].ToString() ?? string.Empty),
            //LockoutEnd = DateTimeOffset.Parse(tableEntity["LockoutEnd"].ToString() ?? string.Empty),
            LockoutEnabled = bool.Parse(tableEntity["LockoutEnabled"].ToString() ?? string.Empty),
            AccessFailedCount = int.Parse(tableEntity["AccessFailedCount"].ToString() ?? string.Empty)
        };
    }
}