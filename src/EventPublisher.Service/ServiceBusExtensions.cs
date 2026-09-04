using System.Diagnostics;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using EventPublisher.Interfaces;

namespace EventPublisher.Service;

public static class ServiceBusExtensions
{
    public static ServiceBusMessage ToServiceBusMessage<T>(this IBaseEvent<T> evt, string sourceName,
        JsonSerializerOptions options, string destination) 
        where T : struct
    {
        var sbm = new ServiceBusMessage(evt.ToBinaryData(options))
        {
            Subject = evt.EventTypeName,
            To = destination,
            ReplyTo = sourceName,
            CorrelationId = evt is IUserEvent<T> uevt? uevt.JobId.Id:null
        };

        if (Activity.Current?.Id != null)
        {
            sbm.ApplicationProperties.Add(IBaseEvent<T>.TraceParent, Activity.Current.Id);
            sbm.ApplicationProperties.Add(IBaseEvent<T>.TraceState, Activity.Current.TraceStateString ?? string.Empty);
        }

        return sbm;
    }


    private static BinaryData ToBinaryData(this object evt, JsonSerializerOptions options)
    {
        var json = JsonSerializer.Serialize(evt, options);
        return BinaryData.FromString(json);
    }
}