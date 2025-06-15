using Azure.Data.Tables;
using VvdKRepositry.Repositries.Contracts.Table.Base;

namespace VvdKRepositry.Repositries.Table.Base;

public abstract class BaseTableRepositry(IBaseTablePersistence tablePersistence):ITableRepositry
{
    protected abstract string TableName { get; }

    public  Task DeleteAllRows()
    {
        return tablePersistence.DeleteAllRows(TableName);
    }

    public  Task DeleteAsync(string partition, string rowkey)
    {
        return tablePersistence.DeleteAsync(TableName, partition, rowkey);
    }

    public  Task<bool> SubmitChangesAsync(List<TableEntity>? additions, List<TableEntity>? updates, List<TableEntity>? deletes)
    {
        return tablePersistence.SubmitChangesAsync(TableName, additions, updates, deletes);
    }

    public  Task<bool> SubmitChangesAsync(List<ITableEntity>? additions, List<ITableEntity>? updates, List<ITableEntity>? deletes)
    {
        return tablePersistence.SubmitChangesAsync(TableName, additions, updates, deletes);
    }

    public  Task<bool> Add(ITableEntity addition)
    {
        return tablePersistence.Add(TableName, addition);
    }

    public  Task<bool> Update(ITableEntity update)
    {
        return tablePersistence.Update(TableName, update);
    }

    public  Task<List<TableEntity>> FetchByFilterAsync(string query, int requestedItemCount, CancellationToken cancellationToken)
    {
        return tablePersistence.FetchByFilterAsync(TableName, query, requestedItemCount, cancellationToken);
    }

    public  Task<List<T>> FetchByFilterAsync<T>(string query, int requestedItemCount, CancellationToken cancellationToken) where T : class, ITableEntity
    {
        return tablePersistence.FetchByFilterAsync<T>(TableName, query, requestedItemCount, cancellationToken);
    }

    public  Task<List<T>> FetchByFilterAsync<T>(string query, int requestedItemCount, Func<TableEntity, T?> filterfunction,
        CancellationToken cancellationToken)
    {
        return tablePersistence.FetchByFilterAsync(TableName, query, requestedItemCount, filterfunction, cancellationToken);
    }

    public  Task<List<T>> FetchByFilterAsync<T>(string query, int requestedItemCount, Func<T, bool> filterfunction,
        CancellationToken cancellationToken) where T : class, ITableEntity
    {
        return tablePersistence.FetchByFilterAsync(TableName, query, requestedItemCount, filterfunction, cancellationToken);
    }

    public  Task<TableEntity?> FetchEntityAsync(string partition, string rowkey, CancellationToken cancellationToken)
    {
        return tablePersistence.FetchEntityAsync(TableName, partition, rowkey, cancellationToken);
    }

    public  Task<T?> FetchEntityAsync<T>(string partition, string rowkey, CancellationToken cancellationToken) where T : class, ITableEntity
    {
        return tablePersistence.FetchEntityAsync<T>(TableName, partition, rowkey, cancellationToken);
    }

    public  Task<List<TableEntity>> FetchPartitionAsync(string partition, int requestedItemCount = 2147483647)
    {
        return tablePersistence.FetchPartitionAsync(TableName, partition, requestedItemCount);
    }

    public  Task<List<T>> FetchPartitionAsync<T>(string partition, int requestedItemCount = 2147483647) where T : class, ITableEntity
    {
        return tablePersistence.FetchPartitionAsync<T>(TableName, partition, requestedItemCount);
    }

    public List<TableEntity> FetchPartition(string partition, int requestedItemCount, CancellationToken cancellationToken)
    {
        return tablePersistence.FetchPartition(TableName, partition, requestedItemCount, cancellationToken);
    }

    public List<T> FetchPartition<T>(string partition, int requestedItemCount, CancellationToken cancellationToken) where T : class, ITableEntity
    {
        return tablePersistence.FetchPartition<T>(TableName, partition, requestedItemCount, cancellationToken);
    }

    public  Task<List<TableEntity>> FetchByPartitionAndPropertyAsync(string partition, string property, string value, int requestedItemCount,
        CancellationToken cancellationToken)
    {
        return tablePersistence.FetchByPartitionAndPropertyAsync(TableName, partition, property, value, requestedItemCount, cancellationToken);
    }

    public  Task<List<T>> FetchByPartitionAndPropertyAsync<T>(string partition, string property, string value, int requestedItemCount,
        CancellationToken cancellationToken) where T : class, ITableEntity
    {
        return tablePersistence.FetchByPartitionAndPropertyAsync<T>(TableName, partition, property, value, requestedItemCount, cancellationToken);
    }

    public  Task<List<TableEntity>> FetchByRowKey(string value, int requestedItemCount, CancellationToken cancellationToken)
    {
        return tablePersistence.FetchByRowKey(TableName, value, requestedItemCount, cancellationToken);
    }

    public  Task<List<T>> FetchByRowKey<T>(string value, int requestedItemCount, CancellationToken cancellationToken) where T : class, ITableEntity
    {
        return tablePersistence.FetchByRowKey<T>(TableName, value, requestedItemCount, cancellationToken);
    }

    public  Task DeleteTableAsync()
    {
        return tablePersistence.DeleteTableAsync(TableName);
    }

    public  Task InitializeAsync()
    {
        return tablePersistence.InitializeAsync(TableName);
    }
    
    
}