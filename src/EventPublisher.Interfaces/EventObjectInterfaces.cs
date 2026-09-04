namespace EventPublisher.Interfaces;

public interface IBaseEvent<T> where T: struct
{
    public const string TraceParent = "traceparent";
    public const string TraceState = "tracestate";
    public string EventTypeName { get; }
    public T? Destination { get; set; }
    public T Subscriptions { get; }
    
    public IBaseEvent<T> CloneBaseEvent(T destination);
}



public interface IUserEvent<T>:IBaseEvent<T> where T : struct
{
    public string UserId { get; init; }
    public Stack<IUserEvent<T>>? FollowUpEvents { get; set; }
    public JobId JobId { get; init; }
    public IUserEvent<T> CloneUserEvent(T destination, JobId? jobId);
}

public enum ResponseToClientModes
{
    Silent,
    Email,
    WebClient
}

public interface IUserEventReportingBack<T>:IUserEvent<T> where T : struct
{
    public ResponseToClientModes ResponseToClientMode { get; set; }
}

public interface IDelayedUserEvent<T>:IUserEvent<T> where T : struct
{
    public DateTimeOffset ExecutionDateTime { get; init; }
}