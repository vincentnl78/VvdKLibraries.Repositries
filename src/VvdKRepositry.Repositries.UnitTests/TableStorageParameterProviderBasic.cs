using VvdKRepositry.Repositries.Contracts;

namespace VvdKRepositry.Repositries.UnitTests;

public class TableStorageParameterProviderBasic() : ITableStorageParameterProvider
{
    public string Id { get; set; } = "default";
    public required string BlobUri { get; init; } ="not set";
    public required string TableUri { get; init; }="not set";
    public string ServiceClientIdentifier => "DefaultUserClient";
    public string TableName =>MakeContainerName(Id);
    public string BlobContainerName => MakeContainerName(Id);
    private static string MakeContainerName(string userid) => "user" + userid.Replace("-", string.Empty);
}