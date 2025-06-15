using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using Serilog;
using VvdKRepositry.Repositries.Contracts.Table.Base;

namespace VvdKRepositry.Repositries.Table.Base;

public abstract class BaseTablePersistence(TableServiceClient client) : IBaseTablePersistence
{
    private const int BatchSize = 100;
    //private TableClient Table => client.GetTableClient(TableName);
    private TableClient GetTableClient(string tableName)
    {
        return client.GetTableClient(tableName);
    } 

    #region Change

    public async Task DeleteAllRows(string tableName)
    {
        var table =GetTableClient(tableName);
        List<TableEntity> tableEntities = [];
        var pageableResult = table.Query<TableEntity>((string)null!, null, null, CancellationToken.None);
        foreach (var page in pageableResult.AsPages()) tableEntities.AddRange(page.Values);

        await SubmitChangesAsync(tableName,null, null, tableEntities);
    }

    public async Task DeleteAsync(string tableName,string partition, string rowkey)
    {
        var table =GetTableClient(tableName);
        await table.DeleteEntityAsync(partition, rowkey);
    }


    public async Task<bool> SubmitChangesAsync(string tableName,List<TableEntity>? additions, List<TableEntity>? updates,
        List<TableEntity>? deletes)
    {
        return await SubmitChangesAsync(tableName,
            additions?.Cast<ITableEntity>().ToList(),
            updates?.Cast<ITableEntity>().ToList(),
            deletes?.Cast<ITableEntity>().ToList());
    }
    
    public async Task<bool> SubmitChangesAsync(string tableName,List<ITableEntity>? additions, List<ITableEntity>? updates,
        List<ITableEntity>? deletes)
    {
        var hasSentItems = false;
        HashSet<string> partitions = [];
        //RemoveDoubles();
        if (additions != null) partitions.UnionWith(additions.Select(t => t.PartitionKey));

        if (updates != null) partitions.UnionWith(updates.Select(t => t.PartitionKey));

        if (deletes != null) partitions.UnionWith(deletes.Select(t => t.PartitionKey));

        Log.Debug("Azure Save: Added {AdditionsCount}, Updated {UpdatesCount}, Deleted {DeletesCount}",
            additions?.Count, updates?.Count, deletes?.Count);

        foreach (var partition in partitions)
        {
            List<TableTransactionAction> batch = [];
            if (additions != null)
            {
                var toBeAdded = additions.Where(t => t.PartitionKey == partition).ToList();
                while (toBeAdded.Count > 0)
                {
                    var addBatch = toBeAdded.Take(BatchSize - batch.Count).ToList();
                    toBeAdded.RemoveRange(0, addBatch.Count);
                    foreach (var transaction in addBatch)
                        batch.Add(new TableTransactionAction(TableTransactionActionType.Add, transaction));

                    if (batch.Count == BatchSize)
                    {
                        hasSentItems = await ExecuteBatch(tableName, batch);
                        batch = [];
                    }
                }
            }

            if (updates != null)
            {
                var toBeUpdatedOperations = updates.Where(t => t.PartitionKey == partition).ToList();


                while (toBeUpdatedOperations.Count > 0)
                {
                    var addBatch =
                        toBeUpdatedOperations.Take(BatchSize - batch.Count).ToList();
                    toBeUpdatedOperations.RemoveRange(0, addBatch.Count);
                    foreach (var updated in addBatch)
                        batch.Add(new TableTransactionAction(TableTransactionActionType.UpsertReplace, updated));

                    if (batch.Count == BatchSize)
                    {
                        hasSentItems = await ExecuteBatch(tableName, batch);
                        batch = [];
                    }
                }
            }

            if (deletes != null)
            {
                var toBeDeleted = deletes
                    .Where(t => t.PartitionKey == partition).ToList();
                while (toBeDeleted.Count > 0)
                {
                    var deleteSet = toBeDeleted.Take(BatchSize - batch.Count).ToList();
                    toBeDeleted.RemoveRange(0, deleteSet.Count);
                    foreach (var tobedeletedTransaction in deleteSet)
                        batch.Add(new TableTransactionAction(TableTransactionActionType.Delete,
                            tobedeletedTransaction));

                    if (batch.Count == BatchSize)
                    {
                        hasSentItems = await ExecuteBatch(tableName,batch);
                        batch = [];
                    }
                }
            }

            if (batch.Count != 0) hasSentItems = await ExecuteBatch(tableName, batch);
        }

        return hasSentItems;
    }

    public Task<bool> Add(string tableName,ITableEntity addition)
    {
        return SubmitChangesAsync(tableName,[addition], null, null);
    }

    public Task<bool> Update(string tablename,ITableEntity update)
    {
        return SubmitChangesAsync(tablename,null,[update], null);
    }


    private async Task<bool> ExecuteBatch(string tablename,List<TableTransactionAction> batch)
    {
        try
        {
            var table =GetTableClient(tablename);
            if (!batch.Any()) 
                return false;
            await table.SubmitTransactionAsync(batch);
            return true;
        }
        catch (RequestFailedException ex)
        {
            Log.Error(ex, "executing batch: {@Batch}", batch);
            throw;
        }
    }

    #endregion

    #region Query

    public Task<TableEntity?> FetchEntityAsync(string tableName,string partition, string rowkey, CancellationToken cancellationToken)
    {
        return FetchEntityAsync<TableEntity>(tableName,partition, rowkey, cancellationToken);
    }
    public async Task<T?> FetchEntityAsync<T>(string tableName,string partition, string rowkey, CancellationToken cancellationToken) where T : class, ITableEntity
    {
        var table =GetTableClient(tableName);
        var nullableResponse = await table.GetEntityIfExistsAsync<T>(partition, rowkey, cancellationToken: cancellationToken);
        if (nullableResponse.HasValue) return nullableResponse.Value;
        return null;
    }

    public Task<List<TableEntity>> FetchByFilterAsync(string tableName,string query, int requestedItemCount, CancellationToken cancellationToken)
    {
        return FetchByFilterAsync<TableEntity>(tableName, query, requestedItemCount, cancellationToken);
    }
    public async Task<List<T>> FetchByFilterAsync<T>(
        string tableName,string query,
        int requestedItemCount,
        CancellationToken cancellationToken
    ) where T : class, ITableEntity
    {
        var table =GetTableClient(tableName);
        List<T> entities = [];
        var pages = table.QueryAsync<T>(
            query,
            1000,
            null,
            cancellationToken);
        await foreach (var page in pages)
        {
            entities.Add(page);
            if (entities.Count >= requestedItemCount)
                break;
        }

        return entities;
    }

    public async Task<List<T>> FetchByFilterAsync<T>(string tableName,string query, int requestedItemCount, Func<TableEntity, T?> filterfunction, CancellationToken cancellationToken)
    {
        var table =GetTableClient(tableName);
        List<T> entities = [];
        var pages = table.QueryAsync<TableEntity>(
            query,
            1000,
            null,
            cancellationToken);
        await foreach (var page in pages)
        {
            var entity = filterfunction(page);
            if (entity != null)
            {
                entities.Add(entity);
                if (entities.Count >= requestedItemCount)
                    break;
            }
        }
        return entities;
    }
    // public Task<List<TableEntity>> FetchByFilterAsync(string query, int requestedItemCount, Func<TableEntity, bool> filterfunction, CancellationToken cancellationToken)
    // {
    //     return FetchByFilterAsync<TableEntity>(query, requestedItemCount, filterfunction, cancellationToken);
    // }
    
    public async Task<List<T>> FetchByFilterAsync<T>(string tableName,string query, int requestedItemCount, Func<T, bool> filterfunction,
        CancellationToken cancellationToken) where T : class, ITableEntity
    {
        var table =GetTableClient(tableName);
        List<T> entities = [];
        var pages = table.QueryAsync<T>(
            query,
            1000,
            null,
            cancellationToken);
        await foreach (var page in pages)
        {
            if (filterfunction(page))
            {
                entities.Add(page);
                if (entities.Count >= requestedItemCount)
                    break;
            }
        }
        return entities;
    }

    public Task<List<TableEntity>> FetchPartitionAsync(string tableName,string partition, int requestedItemCount = 2147483647)
    {
        return FetchPartitionAsync<TableEntity>(tableName,partition, requestedItemCount);
    }
    
    public async Task<List<T>> FetchPartitionAsync<T>(string tableName,string partition, int requestedItemCount = int.MaxValue) where T : class, ITableEntity
    {
        var table =GetTableClient(tableName);
        List<T> entities = [];
        var pages = table.QueryAsync<T>($"PartitionKey eq '{partition}'");
        await foreach (var page in pages)
        {
            entities.Add(page);
            if (entities.Count >= requestedItemCount) break;
        }

        return entities;
    }

    public List<TableEntity> FetchPartition(string tableName,string partition, int requestedItemCount, CancellationToken cancellationToken)
    {
        return FetchPartition<TableEntity>(tableName,partition, requestedItemCount, cancellationToken);
    }

    public List<T> FetchPartition<T>(string tableName,string partition, int requestedItemCount,
        CancellationToken cancellationToken) where T : class, ITableEntity
    {
        var table =GetTableClient(tableName);
        List<T> entities = [];
        var pages = table.Query<T>($"PartitionKey eq '{partition}'", cancellationToken: cancellationToken);
        foreach (var page in pages)
        {
            entities.Add(page);
            if (entities.Count >= requestedItemCount) break;
        }

        return entities;
    }

    public Task<List<TableEntity>> FetchByPartitionAndPropertyAsync(string tableName,string partition, string property, string value, int requestedItemCount,
        CancellationToken cancellationToken)
    {
        return FetchByPartitionAndPropertyAsync<TableEntity>(tableName,partition, property, value, requestedItemCount, cancellationToken);
    }

    public async Task<List<T>> FetchByPartitionAndPropertyAsync<T>(string tableName,string partition, string property, string value, int requestedItemCount, CancellationToken cancellationToken) 
        where T : class, ITableEntity
    {
        var table =GetTableClient(tableName);
        List<T> entities = [];
        var pages = table.QueryAsync<T>($"PartitionKey eq '{partition}' and {property} eq '{value}'",
            cancellationToken: cancellationToken);
        await foreach (var page in pages)
        {
            entities.Add(page);
            if (entities.Count >= requestedItemCount) break;
        }

        return entities;
    }

    public Task<List<TableEntity>> FetchByRowKey(string tableName,string value, int requestedItemCount, CancellationToken cancellationToken)
    {
        return FetchByRowKey<TableEntity>(tableName,value,requestedItemCount, cancellationToken);
    }

    public async Task<List<T>> FetchByRowKey<T>(string tableName,string value, int requestedItemCount,
        CancellationToken cancellationToken) where T : class, ITableEntity
    {
        var table =GetTableClient(tableName);
        List<T> entities = [];
        var pages = table.QueryAsync<T>($"RowKey eq '{value}'", cancellationToken: cancellationToken);
        await foreach (var page in pages)
        {
            entities.Add(page);
            if (entities.Count >= requestedItemCount) break;
        }

        return entities;
    }
    #endregion

    #region Lifetime

    public async Task DeleteTableAsync(string tableName)
    {
        var table =GetTableClient(tableName);
        await table.DeleteAsync();
        Log.Information("Deleted table: {TableName}", tableName);
    }

    public async Task<bool> InitializeAsync(string tableName)
    {
        var tableClient=GetTableClient(tableName);
        try
        {
            Response<TableItem> response = await tableClient.CreateIfNotExistsAsync();
            int statusCode = response.GetRawResponse().Status;
            
            if(statusCode ==409)
            {
                Log.Information("Creating Table {TableName}, already exists", tableName);
                return false;
            }
            
            if (statusCode is 201 or 204)
            {
                Log.Information("Table created: {TableName}", tableName);
                return true;
            }
            Log.Error("Table not created: {TableName} - status code {StatusCode}", tableName,statusCode);
            return false;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Creating container");
            await Task.Delay(5000);
            await tableClient.CreateIfNotExistsAsync();
            return false;
        }
    }

    #endregion
}