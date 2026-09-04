using System.Text;
using System.Text.Json;
using Azure.Storage.Queues;
using EventPublisher.Interfaces;

namespace EventPublisher.Service;

public abstract class AzureStorageQueuePublisher<TDestination>(JsonSerializerOptions jsonSerializerOptions)
    : IAzureStorageQueuePublisher<TDestination> where TDestination : struct
{
    private readonly Dictionary<string, QueueClient> _queues = [];

    

    protected abstract QueueServiceClient QueueServiceClient {get;}

    public async Task PublishAsync<T>(T item) where T : IBaseEvent<TDestination>
    {
        var msg = JsonSerializer.Serialize((object) item, jsonSerializerOptions);
        var base64Msg = Convert.ToBase64String(Encoding.UTF8.GetBytes(msg));

        foreach (var flag in GetServiceTypes(item.Subscriptions))
        {
            var queue = GetQueueForService(flag);
            var utcNow = DateTimeOffset.UtcNow;
            var delay = item is IDelayedUserEvent<TDestination> delayedUserEvent
                ? delayedUserEvent.ExecutionDateTime - utcNow
                : TimeSpan.Zero;
            // Clamp to zero if targetTime is in the past
            if (delay < TimeSpan.Zero)
                delay = TimeSpan.Zero;
            await queue.SendMessageAsync(base64Msg, delay);
        }
        /*var queue = item.EventTypeName switch
        {
            BankEvents.SyncEnabledBanks.Name => BankSyncQueue,
            _ => throw new NotSupportedException($"Event type {item.EventTypeName} is not supported.")
        };*/
    }

    public async Task PublishAsync<T>(List<T> evt) where T : IBaseEvent<TDestination>
    {
        foreach (var baseEvent in evt)
        {
            await PublishAsync(baseEvent); 
        }
    }

    public async Task Cleanup(TDestination serviceType)
    {
        var queue = GetQueueForService(serviceType);
        await queue.ClearMessagesAsync();
    }

    public abstract Task InitStorageAsync();

    

    protected abstract string QueueName(TDestination serviceType);

    protected QueueClient GetQueueForService(TDestination serviceType)
    {
        var queueString = QueueName(serviceType);
        if (_queues.TryGetValue(queueString, out var queue)) return queue;
        queue = QueueServiceClient.GetQueueClient(queueString);
        _queues[queueString] = queue;
        return queue;
    }

    protected abstract IEnumerable<TDestination> GetServiceTypes(TDestination serviceType);
}