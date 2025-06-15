using Azure.Data.Tables;

namespace VvdKRepositry.Repositries.Contracts.Table.Base;

public interface ITableRepositry
{
    #region Change
    Task DeleteAllRows();
    Task DeleteAsync(string partition, string rowkey);
    Task<bool> SubmitChangesAsync(List<TableEntity>? additions, List<TableEntity>? updates, List<TableEntity>? deletes);
    Task<bool> SubmitChangesAsync(List<ITableEntity>? additions, List<ITableEntity>? updates, List<ITableEntity>? deletes);
    Task<bool> Add(ITableEntity addition);
    Task<bool> Update(ITableEntity update);
    #endregion

    #region Query

    Task<List<TableEntity>> FetchByFilterAsync(string query, int requestedItemCount, CancellationToken cancellationToken);
    Task<List<T>> FetchByFilterAsync<T>(string query, int requestedItemCount, CancellationToken cancellationToken)
        where T : class, ITableEntity;

    Task<List<T>> FetchByFilterAsync<T>(string query, int requestedItemCount, Func<TableEntity, T?> filterfunction, CancellationToken cancellationToken);
    Task<List<T>> FetchByFilterAsync<T>(string query, int requestedItemCount,Func<T,bool> filterfunction, CancellationToken cancellationToken)
        where T : class, ITableEntity;
    
    
    Task<TableEntity?> FetchEntityAsync(string partition, string rowkey, CancellationToken cancellationToken);
    Task<T?> FetchEntityAsync<T>(string partition, string rowkey, CancellationToken cancellationToken)
        where T : class, ITableEntity;

    Task<List<TableEntity>> FetchPartitionAsync(string partition, int requestedItemCount = int.MaxValue);
    Task<List<T>> FetchPartitionAsync<T>(string partition, int requestedItemCount = int.MaxValue)
        where T : class, ITableEntity;

    List<TableEntity> FetchPartition(string partition, int requestedItemCount, CancellationToken cancellationToken);
    List<T> FetchPartition<T>(string partition, int requestedItemCount, CancellationToken cancellationToken) 
        where T : class, ITableEntity;

    Task<List<TableEntity>> FetchByPartitionAndPropertyAsync(string partition, string property, string value, int requestedItemCount, CancellationToken cancellationToken);
    Task<List<T>> FetchByPartitionAndPropertyAsync<T>(string partition, string property, string value, int requestedItemCount, CancellationToken cancellationToken) 
        where T : class, ITableEntity;

    Task<List<TableEntity>> FetchByRowKey(string value, int requestedItemCount, CancellationToken cancellationToken);
    Task<List<T>> FetchByRowKey<T>(string value, int requestedItemCount, CancellationToken cancellationToken) 
        where T : class, ITableEntity;
    #endregion

    #region Lifetime

    Task DeleteTableAsync();

    Task InitializeAsync();

    #endregion
}