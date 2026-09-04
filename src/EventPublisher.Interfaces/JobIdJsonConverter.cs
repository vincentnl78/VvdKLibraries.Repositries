using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventPublisher.Interfaces;

public class JobIdJsonConverter : JsonConverter<JobId>
{
    public override JobId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString() ?? throw new JsonException("JobId is null");
        return new JobId(value);
    }

    public override void Write(Utf8JsonWriter writer, JobId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Id);
    }
}

/*public class JobUpdateIdJsonConverter : JsonConverter<JobUpdateId>
{
    public override JobUpdateId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString() ?? throw new JsonException("JobId is null");
        return new JobUpdateId(value);
    }

    public override void Write(Utf8JsonWriter writer, JobUpdateId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Id);
    }
}*/