using Azure.Data.Tables;

namespace VvdKRepositry.Repositries.Contracts.Table.Base;

public interface IBaseTablePersistence
{
    #region Change
    Task DeleteAllRows(string tableName);
    Task DeleteAsync(string tableName,string partition, string rowkey);
    Task<bool> SubmitChangesAsync(string tableName,List<TableEntity>? additions, List<TableEntity>? updates, List<TableEntity>? deletes);
    Task<bool> SubmitChangesAsync(string tableName,List<ITableEntity>? additions, List<ITableEntity>? updates, List<ITableEntity>? deletes);
    Task<bool> Add(string tableName,ITableEntity addition);
    Task<bool> Update(string tableName,ITableEntity update);
    #endregion

    #region Query

    Task<List<TableEntity>> FetchByFilterAsync(string tableName,string query, int requestedItemCount, CancellationToken cancellationToken);
    Task<List<T>> FetchByFilterAsync<T>(string tableName,string query, int requestedItemCount, CancellationToken cancellationToken)
        where T : class, ITableEntity;

    Task<List<T>> FetchByFilterAsync<T>(string tableName,string query, int requestedItemCount, Func<TableEntity, T?> filterfunction, CancellationToken cancellationToken);
    Task<List<T>> FetchByFilterAsync<T>(string tableName,string query, int requestedItemCount,Func<T,bool> filterfunction, CancellationToken cancellationToken)
        where T : class, ITableEntity;
    
    
    Task<TableEntity?> FetchEntityAsync(string tableName,string partition, string rowkey, CancellationToken cancellationToken);
    Task<T?> FetchEntityAsync<T>(string tableName,string partition, string rowkey, CancellationToken cancellationToken)
        where T : class, ITableEntity;

    Task<List<TableEntity>> FetchPartitionAsync(string tableName,string partition, int requestedItemCount = int.MaxValue);
    Task<List<T>> FetchPartitionAsync<T>(string tableName,string partition, int requestedItemCount = int.MaxValue)
        where T : class, ITableEntity;

    List<TableEntity> FetchPartition(string tableName,string partition, int requestedItemCount, CancellationToken cancellationToken);
    List<T> FetchPartition<T>(string tableName,string partition, int requestedItemCount, CancellationToken cancellationToken) 
        where T : class, ITableEntity;

    Task<List<TableEntity>> FetchByPartitionAndPropertyAsync(string tableName,string partition, string property, string value, int requestedItemCount, CancellationToken cancellationToken);
    Task<List<T>> FetchByPartitionAndPropertyAsync<T>(string tableName, string partition, string property, string value, int requestedItemCount, CancellationToken cancellationToken) 
        where T : class, ITableEntity;

    Task<List<TableEntity>> FetchByRowKey(string tableName,string value, int requestedItemCount, CancellationToken cancellationToken);
    Task<List<T>> FetchByRowKey<T>(string tableName,string value, int requestedItemCount, CancellationToken cancellationToken) 
        where T : class, ITableEntity;
    #endregion

    #region Lifetime

    Task DeleteTableAsync(string tableName);

    Task<bool> InitializeAsync(string tableName);

    #endregion
}