using EventPublisher.Interfaces;

namespace EventPublisher.Service;

/// <summary>
/// Delayed messages will be queued, the rest will be published
/// </summary>
/// <param name="serviceBusPublisher"></param>
/// <param name="queuePublisher"></param>
public class AzurePublisherQueueAndBus<TDestination>(
    IAzureServiceBusPublisher<TDestination> serviceBusPublisher, 
    IAzureStorageQueuePublisher<TDestination> queuePublisher)
    :IBaseEventPublisher<TDestination> 
    where TDestination : struct, Enum
{
    public Task Cleanup(TDestination serviceType)
    {
        return serviceBusPublisher.Cleanup(serviceType);
    }

    public Task PublishAsync<T>(T evt) where T : IBaseEvent<TDestination>
    {
        if (evt is IDelayedUserEvent<TDestination> delayedUserEvent)
        {
            return queuePublisher.PublishAsync(delayedUserEvent);
        }
        return serviceBusPublisher.PublishAsync(evt);
    }

    public async Task PublishAsync<T>(List<T> evt) where T : IBaseEvent<TDestination>
    {
        List<IDelayedUserEvent<TDestination>> delayedUserEvents = new();
        List<IBaseEvent<TDestination>> nonDelayedUserEvents = new();
        foreach (var userEvent in evt)
        {
            if (userEvent is IDelayedUserEvent<TDestination> delayedUserEvent)
            {
                delayedUserEvents.Add(delayedUserEvent);
            }
            else
            {
                nonDelayedUserEvents.Add(userEvent);
            }
        }
        
        if (delayedUserEvents.Count != 0)
        {
            foreach (var delayedUserEvent in delayedUserEvents)
            {
                await queuePublisher.PublishAsync(delayedUserEvent);    
            }
        }
        if (nonDelayedUserEvents.Count != 0)
        {
            await serviceBusPublisher.PublishAsync(nonDelayedUserEvents);
        }
    }
}