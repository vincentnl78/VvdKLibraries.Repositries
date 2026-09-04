using System.Text.Json.Serialization;

namespace EventPublisher.Interfaces;

[JsonConverter(typeof(JobIdJsonConverter))]
public record JobId(string Id)
{
    [JsonIgnore]
    public string MessageTypePart
    {
        get
        {
            var parts = Id.Split('_', 2);
            return parts.Length == 2 ? parts[1] : string.Empty;
        }
    }

    [JsonIgnore]
    public string GuidPart
    {
        get
        {
            var parts = Id.Split('_', 2);
            return parts[0];
        }
    }

    [JsonIgnore]
    public DateOnly Date
    {
        get
        {
            var guid = Guid.Parse(GuidPart);
            var dateTime = GetUuid7DateTime(guid);
            return DateOnly.FromDateTime(dateTime.DateTime);
        }
    }

    public static JobId Create(string messageType)
    {
        var guid = Guid.CreateVersion7();
        return new JobId($"{guid}_{messageType}");
    }

    public static JobId Create(string guid, string messageType)
    {
        return new JobId($"{guid}_{messageType}");
    }


    private static DateTimeOffset GetUuid7DateTime(Guid guid)
    {
        if (guid.Version != 7)
            throw new ArgumentException("GUID is not version 7.", nameof(guid));

        byte[] bytes = guid.ToByteArray();

        // Assemble 48‑bit timestamp as big‑endian
        long unixMs =
            ((long)bytes[3] << 40) |
            ((long)bytes[2] << 32) |
            ((long)bytes[1] << 24) |
            ((long)bytes[0] << 16) |
            ((long)bytes[5] << 8) |
            bytes[4];

        return DateTimeOffset.FromUnixTimeMilliseconds(unixMs);
    }
}