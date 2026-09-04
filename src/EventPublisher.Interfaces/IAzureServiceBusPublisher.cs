namespace EventPublisher.Interfaces;

public interface IAzureServiceBusPublisher<T>:IBaseEventPublisher<T> where T : struct;