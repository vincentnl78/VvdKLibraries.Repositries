namespace EventPublisher.Interfaces;

public interface IBaseEventPublisher<TDestination> where TDestination : struct
{
    Task PublishAsync<T>(T evt) where T : IBaseEvent<TDestination> ;
    Task PublishAsync<T>(List<T> evt) where T : IBaseEvent<TDestination>;
    Task Cleanup(TDestination serviceType);
}