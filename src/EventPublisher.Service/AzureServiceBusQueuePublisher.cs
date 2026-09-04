using System.Text.Json;
using Azure.Messaging.ServiceBus;
using EventPublisher.Interfaces;
using Serilog;

namespace EventPublisher.Service;

public record ServiceBusOptions
{
    public required string ServiceName { get; set; }
}

public abstract class AzureServiceBusQueuePublisher<TDestination>(
    JsonSerializerOptions jsonSerializerOptions,
    ServiceBusClient client,
    ServiceBusOptions options
) : IAzureServiceBusPublisher<TDestination> where TDestination : struct
{
    public Task PublishAsync<T>(T evt) where T : IBaseEvent<TDestination>
    {
        return PublishAsync([evt]);
    }

    public async Task PublishAsync<T>(List<T> events) where T : IBaseEvent<TDestination>
    {
        Dictionary<TDestination, List<IBaseEvent<TDestination>>> sendBatch = [];
        foreach (var baseEvent in events)
            AddEventToQueue(baseEvent, sendBatch);
        await PublishToServiceBus(sendBatch);
    }

    public async Task Cleanup(TDestination serviceType)
    {
        foreach (var service in GetServiceTypes(serviceType))
        {
            var queueName = GetQueueName(service);
            var receiver = client.CreateReceiver(queueName, new ServiceBusReceiverOptions
            {
                SubQueue = SubQueue.DeadLetter,
                ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
            });
            while (true)
            {
                var messages = await receiver.ReceiveMessagesAsync(100, TimeSpan.FromMilliseconds(500));
                Log.Information("Purged {Count} deadletter items from {QueueName}", messages.Count, queueName);
                if (messages.Count == 0) break;
            }
        }
    }

    protected abstract IEnumerable<TDestination> GetServiceTypes(TDestination serviceType);
    protected abstract string GetQueueName(TDestination service);

    private void AddEventToQueue<T>(T evt, Dictionary<TDestination, List<IBaseEvent<TDestination>>> sendBatch)
        where T : IBaseEvent<TDestination>
    {
        if (evt.Destination == null)
        {
            if (evt is IUserEventReportingBack<TDestination> userevt)
            {
                var first = true;
                foreach (var service in GetServiceTypes(evt.Subscriptions))
                    if (first)
                    {
                        first = false;
                        userevt.Destination = service;
                    }
                    else
                    {
                        userevt.FollowUpEvents ??= new Stack<IUserEvent<TDestination>>();
                        userevt.FollowUpEvents.Push(userevt.CloneUserEvent(service, null));
                    }

                if (userevt.Destination != null)
                {
                    AddEvent(userevt, userevt.Destination.Value);    
                }
            }
            else
            {
                foreach (var service in GetServiceTypes(evt.Subscriptions))
                    AddEvent(evt.CloneBaseEvent(service), service);
            }
        }
        else
        {
            AddEvent(evt, evt.Destination.Value);
        }

        void AddEvent(IBaseEvent<TDestination> bevt, TDestination service)
        {
            if (!sendBatch.ContainsKey(service))
                sendBatch.Add(service, [bevt]);
            else
                sendBatch[service].Add(bevt);
        }
    }

    private async Task PublishToServiceBus(Dictionary<TDestination, List<IBaseEvent<TDestination>>> sendBatch)
    {
        foreach (var batch in sendBatch)
            await PublishToServiceBus(batch.Value, GetQueueName(batch.Key));
    }

    private async Task PublishToServiceBus<T>(List<T> events, string queuename) where T : IBaseEvent<TDestination>
    {
        var sender = client.CreateSender(queuename);
        var messages =
            events.Select(e => e.ToServiceBusMessage(options.ServiceName, jsonSerializerOptions, queuename));

        if (events.Count == 1)
        {
            var evt = events[0];
            if (evt is IUserEvent<TDestination> uevt)
                Log.Information("Publishing event {@Event} with Id {JobId} to {QueueName}", events[0], uevt.JobId.Id,
                    queuename);
            else
                Log.Information("Publishing base event {@Event} to {QueueName}", events[0], queuename);
        }
        else
        {
            Log.Information("Publishing {Count} events to {QueueName}", events.Count, queuename);
        }

        try
        {
            //create service bus batches and send
            var batch = await sender.CreateMessageBatchAsync();
            foreach (var message in messages)
                if (!batch.TryAddMessage(message))
                {
                    await sender.SendMessagesAsync(batch);
                    batch = await sender.CreateMessageBatchAsync();
                    if (!batch.TryAddMessage(message)) throw new Exception("Message too large to send");
                }

            await sender.SendMessagesAsync(batch);
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to publish events to {QueueName}", queuename);
            throw;
        }
    }
}