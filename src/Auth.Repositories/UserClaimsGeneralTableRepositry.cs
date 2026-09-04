using System.Security.Claims;
using Auth.Interfaces;
using Auth.Interfaces.Storage;
using Azure.Data.Tables;
using VvdKRepositry.Repositries.Table.Base;

namespace Auth.Repositories;

public class UserClaimsGeneralGeneralTableRepositry(IIdentityTablePersistence persistence)
    : BaseTableRepositryWithCreationNotifiers(persistence), IUserClaimsGeneralTableRepositry
{
    protected override string TableName => "UserClaims";

    public async Task<IList<Claim>> GetClaimsAsync(string userId)
    {
        var claims = await FetchPartitionAsync(userId);
        return claims.Select(Convert).ToList();
    }

    public async Task AddAsync(string userId, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        await SubmitChangesAsync(claims.Select(claim => Convert(claim, userId)).ToList(), null, null);
    }

    public async Task DeleteAsync(string userId, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        var deletes = claims.Select(claim => new TableEntity(userId, claim.Type)).ToList();
        await SubmitChangesAsync(null, null, deletes);
    }

    public async Task<IList<string>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
    {
        var pages = await FetchByRowKey(claim.Type, int.MaxValue, cancellationToken);
        return pages.Select(p => p.PartitionKey).ToList();
    }

    private static TableEntity Convert(Claim claim, string userid)
    {
        return new TableEntity(userid, claim.Type)
        {
            ["Value"] = claim.Value
        };
    }

    private static Claim Convert(TableEntity entity)
    {
        return new Claim(entity.RowKey, entity["Value"].ToString() ?? string.Empty);
    }
}