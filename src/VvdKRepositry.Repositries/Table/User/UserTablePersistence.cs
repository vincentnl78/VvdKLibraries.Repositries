using Azure.Data.Tables;
using Microsoft.Extensions.Azure;
using Serilog;
using VvdKRepositry.Repositries.Contracts;
using VvdKRepositry.Repositries.Contracts.Notifications.Creation;
using VvdKRepositry.Repositries.Contracts.Table.User;
using VvdKRepositry.Repositries.Table.Base;

namespace VvdKRepositry.Repositries.Table.User;

public class UserTablePersistence(
    IIdProvider idProvider, 
    IAzureClientFactory<TableServiceClient> factory)
    : BaseTablePersistence(factory.CreateClient(idProvider.ServiceClientIdentifier)), IUserTablePersistence
{
    public async Task Handle(CreateUserPersistenceSetupNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            await InitializeAsync(idProvider.TableName);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Creating Table {TableName}", idProvider.TableName);
        }
    }

    public async  Task Handle(DeleteUserPersistenceNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            await DeleteTableAsync(idProvider.TableName);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Deleting Table {TableName}", idProvider.TableName);
        }
    }

    public  Task DeleteAllRows()
    {
        return base.DeleteAllRows(idProvider.TableName);
    }

    public  Task DeleteAsync(string partition, string rowkey)
    {
        return base.DeleteAsync(idProvider.TableName,partition, rowkey );
    }

    public  Task<bool> SubmitChangesAsync(List<TableEntity>? additions, List<TableEntity>? updates, List<TableEntity>? deletes)
    {
        return base.SubmitChangesAsync(idProvider.TableName, additions, updates, deletes);
    }

    public  Task<bool> SubmitChangesAsync(List<ITableEntity>? additions, List<ITableEntity>? updates, List<ITableEntity>? deletes)
    {
        return base.SubmitChangesAsync(idProvider.TableName, additions, updates, deletes);
    }

    public  Task<bool> Add(ITableEntity addition)
    {
        return base.Add(idProvider.TableName, addition);
    }

    public  Task<bool> Update(ITableEntity update)
    {
        return base.Update(idProvider.TableName, update);
    }

    public  Task<List<TableEntity>> FetchByFilterAsync(string query, int requestedItemCount, CancellationToken cancellationToken)
    {
        return base.FetchByFilterAsync(idProvider.TableName, query, requestedItemCount, cancellationToken);
    }

    public  Task<List<T>> FetchByFilterAsync<T>(string query, int requestedItemCount, CancellationToken cancellationToken) where T : class, ITableEntity
    {
        return base.FetchByFilterAsync<T>(idProvider.TableName, query, requestedItemCount, cancellationToken);
    }

    public  Task<List<T>> FetchByFilterAsync<T>(string query, int requestedItemCount, Func<TableEntity, T?> filterfunction,
        CancellationToken cancellationToken)
    {
        return base.FetchByFilterAsync(idProvider.TableName, query, requestedItemCount, filterfunction, cancellationToken);
    }

    public  Task<List<T>> FetchByFilterAsync<T>(string query, int requestedItemCount, Func<T, bool> filterfunction,
        CancellationToken cancellationToken) where T : class, ITableEntity
    {
        return base.FetchByFilterAsync(idProvider.TableName, query, requestedItemCount, filterfunction, cancellationToken);
    }

    public  Task<TableEntity?> FetchEntityAsync(string partition, string rowkey, CancellationToken cancellationToken)
    {
        return base.FetchEntityAsync(idProvider.TableName, partition, rowkey, cancellationToken);
    }

    public  Task<T?> FetchEntityAsync<T>(string partition, string rowkey, CancellationToken cancellationToken) where T : class, ITableEntity
    {
        return base.FetchEntityAsync<T>(idProvider.TableName, partition, rowkey, cancellationToken);
    }

    public  Task<List<TableEntity>> FetchPartitionAsync(string partition, int requestedItemCount = 2147483647)
    {
        return base.FetchPartitionAsync(idProvider.TableName, partition, requestedItemCount);
    }

    public  Task<List<T>> FetchPartitionAsync<T>(string partition, int requestedItemCount = 2147483647) where T : class, ITableEntity
    {
        return base.FetchPartitionAsync<T>(idProvider.TableName, partition, requestedItemCount);
    }

    public List<TableEntity> FetchPartition(string partition, int requestedItemCount, CancellationToken cancellationToken)
    {
        return base.FetchPartition(idProvider.TableName, partition, requestedItemCount,cancellationToken);
    }

    public List<T> FetchPartition<T>(string partition, int requestedItemCount, CancellationToken cancellationToken) where T : class, ITableEntity
    {
        return base.FetchPartition<T>(idProvider.TableName, partition, requestedItemCount,cancellationToken);
    }

    public  Task<List<TableEntity>> FetchByPartitionAndPropertyAsync(string partition, string property, string value, int requestedItemCount,
        CancellationToken cancellationToken)
    {
        return base.FetchByPartitionAndPropertyAsync(idProvider.TableName, partition, property, value, requestedItemCount, cancellationToken);
    }

    public  Task<List<T>> FetchByPartitionAndPropertyAsync<T>(string partition, string property, string value, int requestedItemCount,
        CancellationToken cancellationToken) where T : class, ITableEntity
    {
        return base.FetchByPartitionAndPropertyAsync<T>(idProvider.TableName, partition, property, value, requestedItemCount, cancellationToken);
    }

    public  Task<List<TableEntity>> FetchByRowKey(string value, int requestedItemCount, CancellationToken cancellationToken)
    {
        return base.FetchByRowKey(idProvider.TableName, value, requestedItemCount, cancellationToken);
    }

    public  Task<List<T>> FetchByRowKey<T>(string value, int requestedItemCount, CancellationToken cancellationToken) where T : class, ITableEntity
    {
        return base.FetchByRowKey<T>(idProvider.TableName, value, requestedItemCount, cancellationToken);
    }

    public  Task DeleteTableAsync()
    {
        return base.DeleteTableAsync(idProvider.TableName);
    }

    public  Task InitializeAsync()
    {
        return base.InitializeAsync(idProvider.TableName);
    }
}