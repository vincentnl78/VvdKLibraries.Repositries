using VvdKRepositry.Repositries.Contracts.Blob.Base;

namespace Auth.Interfaces.Storage;

public interface IIdentityBlobPersistence : IBaseBlobPersistence
{
    static string StorageServiceIdentifier => "IdentityBlobPersistence";
}