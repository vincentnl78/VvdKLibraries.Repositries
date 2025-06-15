namespace VvdKRepositry.Repositries.Contracts;

public interface IIdProvider
{
    public string Id { get; set; }
    public string BlobUri { get; }
    public string TableUri { get; }
    public string ServiceClientIdentifier { get; }
    public string TableName { get; }
    public string BlobContainerName { get; }
    public static string MakeContainerName(string userid) => "user" + userid.Replace("-", string.Empty);
}