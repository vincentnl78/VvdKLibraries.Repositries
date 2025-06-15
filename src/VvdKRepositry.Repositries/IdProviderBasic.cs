using VvdKRepositry.Repositries.Contracts;

namespace VvdKRepositry.Repositries;

public class IdProviderBasic() : IIdProvider
{
    public string Id { get; set; } = "default";
    public required string BlobUri { get; set; } ="not set";
    public required string TableUri { get; set; }="not set";
    public string ServiceClientIdentifier => "DefaultUserClient";
    public string TableName =>IIdProvider.MakeContainerName(Id);
    public string BlobContainerName => IIdProvider.MakeContainerName(Id);
}