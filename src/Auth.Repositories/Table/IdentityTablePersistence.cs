using Auth.Interfaces.Storage;
using Azure.Data.Tables;
using Microsoft.Extensions.Azure;
using VvdKRepositry.Repositries.Table.Base;

namespace Auth.Repositories.Table;

public class IdentityTablePersistence(
    IAzureClientFactory<TableServiceClient> factory
)
    : BaseTablePersistence(factory.CreateClient(IIdentityTablePersistence.StorageServiceIdentifier)),
        IIdentityTablePersistence
{
}