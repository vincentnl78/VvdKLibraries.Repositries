using System.Security.Claims;
using Azure.Data.Tables;

namespace Auth.Repositories;

public static class EntityExtensions
{
    public static TableEntity ToEntity(this Claim claim, string userId)
    {
        var te = new TableEntity(userId, claim.Type)
        {
            ["ClaimValue"] = claim.Value
        };
        return te;
    }

    public static Claim ToClaim(this TableEntity entity)
    {
        return new Claim(entity.RowKey, entity["ClaimValue"].ToString() ?? string.Empty);
    }
}