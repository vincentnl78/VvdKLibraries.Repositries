using Azure.Data.Tables;
using VvdKRepositry.Repositries.Contracts.Table.User;

namespace VvdKRepositry.Repositries.Table.User;

public abstract class UserTableRepositry(
    IUserTablePersistence userTablePersistence)
    : IUserTableRepositry
{
    public  Task DeleteAllRows()
    {
        return userTablePersistence.DeleteAllRows();
    }

    public  Task DeleteAsync(string partition, string rowkey)
    {
        return userTablePersistence.DeleteAsync(partition, rowkey);
    }

    public  Task<bool> SubmitChangesAsync(List<TableEntity>? additions, List<TableEntity>? updates, List<TableEntity>? deletes)
    {
        return userTablePersistence.SubmitChangesAsync(additions, updates, deletes);
    }

    public  Task<bool> SubmitChangesAsync(List<ITableEntity>? additions, List<ITableEntity>? updates, List<ITableEntity>? deletes)
    {
        return userTablePersistence.SubmitChangesAsync(additions, updates, deletes);
    }

    public  Task<bool> Add(ITableEntity addition)
    {
        return userTablePersistence.Add(addition);
    }

    public  Task<bool> Update(ITableEntity update)
    {
        return userTablePersistence.Update(update);
    }

    public  Task<List<TableEntity>> FetchByFilterAsync(string query, int requestedItemCount, CancellationToken cancellationToken)
    {
        return userTablePersistence.FetchByFilterAsync(query, requestedItemCount, cancellationToken);
    }

    public  Task<List<T>> FetchByFilterAsync<T>(string query, int requestedItemCount, CancellationToken cancellationToken) where T : class, ITableEntity
    {
        return userTablePersistence.FetchByFilterAsync<T>(query, requestedItemCount, cancellationToken);
    }

    public  Task<List<T>> FetchByFilterAsync<T>(string query, int requestedItemCount, Func<TableEntity, T?> filterfunction,
        CancellationToken cancellationToken)
    {
        return userTablePersistence.FetchByFilterAsync(query, requestedItemCount, filterfunction, cancellationToken);
    }

    public  Task<List<T>> FetchByFilterAsync<T>(string query, int requestedItemCount, Func<T, bool> filterfunction,
        CancellationToken cancellationToken) where T : class, ITableEntity
    {
        return userTablePersistence.FetchByFilterAsync(query, requestedItemCount, filterfunction, cancellationToken);
    }

    public  Task<TableEntity?> FetchEntityAsync(string partition, string rowkey, CancellationToken cancellationToken)
    {
        return userTablePersistence.FetchEntityAsync(partition, rowkey, cancellationToken);
    }

    public  Task<T?> FetchEntityAsync<T>(string partition, string rowkey, CancellationToken cancellationToken) where T : class, ITableEntity
    {
        return userTablePersistence.FetchEntityAsync<T>(partition, rowkey, cancellationToken);
    }

    public  Task<List<TableEntity>> FetchPartitionAsync(string partition, int requestedItemCount = 2147483647)
    {
        return userTablePersistence.FetchPartitionAsync(partition, requestedItemCount);
    }

    public  Task<List<T>> FetchPartitionAsync<T>(string partition, int requestedItemCount = 2147483647) where T : class, ITableEntity
    {
        return userTablePersistence.FetchPartitionAsync<T>(partition, requestedItemCount);
    }

    public List<TableEntity> FetchPartition(string partition, int requestedItemCount, CancellationToken cancellationToken)
    {
        return userTablePersistence.FetchPartition(partition, requestedItemCount, cancellationToken);
    }

    public List<T> FetchPartition<T>(string partition, int requestedItemCount, CancellationToken cancellationToken) where T : class, ITableEntity
    {
        return userTablePersistence.FetchPartition<T>(partition, requestedItemCount, cancellationToken);
    }

    public  Task<List<TableEntity>> FetchByPartitionAndPropertyAsync(string partition, string property, string value, int requestedItemCount,
        CancellationToken cancellationToken)
    {
        return userTablePersistence.FetchByPartitionAndPropertyAsync(partition, property, value, requestedItemCount, cancellationToken);
    }

    public  Task<List<T>> FetchByPartitionAndPropertyAsync<T>(string partition, string property, string value, int requestedItemCount,
        CancellationToken cancellationToken) where T : class, ITableEntity
    {
        return userTablePersistence.FetchByPartitionAndPropertyAsync<T>(partition, property, value, requestedItemCount, cancellationToken);
    }

    public  Task<List<TableEntity>> FetchByRowKey(string value, int requestedItemCount, CancellationToken cancellationToken)
    {
        return userTablePersistence.FetchByRowKey(value, requestedItemCount, cancellationToken);
    }

    public  Task<List<T>> FetchByRowKey<T>(string value, int requestedItemCount, CancellationToken cancellationToken) where T : class, ITableEntity
    {
        return userTablePersistence.FetchByRowKey<T>(value, requestedItemCount, cancellationToken);
    }

    public  Task DeleteTableAsync()
    {
        return userTablePersistence.DeleteTableAsync();
    }

    public  Task InitializeAsync()
    {
        return userTablePersistence.InitializeAsync();
    }
}