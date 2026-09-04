namespace EventPublisher.Interfaces;


public interface IAzureStorageQueuePublisher<TDestination>:IBaseEventPublisher<TDestination> 
    where TDestination : struct
{
    Task InitStorageAsync();
}