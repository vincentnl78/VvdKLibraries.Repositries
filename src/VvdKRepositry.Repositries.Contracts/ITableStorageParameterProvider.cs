namespace VvdKRepositry.Repositries.Contracts;

public interface IUserIdProvider
{
    public string Id { get; set; }
}

public class UserIdProvider:IUserIdProvider
{
    public string Id { get; set; } ="not set";
}


public interface ITableStorageParameterProvider
{
    public string Id { get; }
    public string TableUri { get; }
    public string ServiceClientIdentifier { get; }
    public string TableName { get; }
}

public interface IBlobStorageParameterProvider
{
    public string Id { get; }
    public string BlobUri { get; }
    public string ServiceClientIdentifier { get; }
    public string BlobContainerName { get; }
}