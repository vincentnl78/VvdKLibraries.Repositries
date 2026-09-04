using Auth.Interfaces;
using Auth.Interfaces.Storage;
using Auth.Objects;
using Azure.Data.Tables;
using VvdKRepositry.Repositries.Table.Base;

namespace Auth.Repositories;

public class RefreshTokensGeneralTableRepositry(IIdentityTablePersistence persistence, TimeProvider timeProvider)
    : BaseTableRepositryWithCreationNotifiers(persistence), IRefreshTokensTableRepositry
{
    protected override string TableName => "RefreshTokens";

    public async Task<List<RefreshToken>> FetchAsync(string userid, CancellationToken cancellationToken)
    {
        var entities = await FetchPartitionAsync(userid);
        return entities.Select(Convert).ToList();
    }

    public async Task<RefreshToken?> FetchAsync(string userid, string tokenid, CancellationToken cancellationToken)
    {
        var entity = await FetchEntityAsync(userid, tokenid, cancellationToken);
        return entity is null ? null : Convert(entity);
    }

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        await SubmitChangesAsync([Convert(refreshToken)], null, null);
    }

    public async Task CleanupAsync(CancellationToken cancellationToken)
    {
        var filter = TableClient.CreateQueryFilter<RefreshToken>(t => t.Expiration > timeProvider.GetUtcNow());
        var tokens = await FetchByFilterAsync(filter, int.MaxValue, cancellationToken);
        await SubmitChangesAsync(null, null, tokens);
    }

    private TableEntity Convert(RefreshToken refreshToken)
    {
        return new TableEntity(refreshToken.UserId, refreshToken.Id)
        {
            ["Expiration"] = refreshToken.Expiration,
            ["Token"] = refreshToken.Token,
        };
    }

    private RefreshToken Convert(TableEntity refreshToken)
    {
        return new RefreshToken
        {
            UserId = refreshToken.PartitionKey,
            Token = refreshToken["Token"].ToString() ?? string.Empty,
            Expiration = DateTimeOffset.Parse(refreshToken["Expiration"].ToString() ?? string.Empty),
            Id = refreshToken.RowKey
        };
    }
}