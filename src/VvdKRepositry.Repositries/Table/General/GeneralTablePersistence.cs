using Azure.Data.Tables;
using Microsoft.Extensions.Azure;
using VvdKRepositry.Repositries.Contracts.Blob.General;
using VvdKRepositry.Repositries.Contracts.Table.General;
using VvdKRepositry.Repositries.Table.Base;

namespace VvdKRepositry.Repositries.Table.General;

public class GeneralTablePersistence(IAzureClientFactory<TableServiceClient> factory)
    : BaseTablePersistence(factory.CreateClient(IGeneralBlobPersistence.StorageServiceIdentifier)),
        IGeneralTablePersistence;