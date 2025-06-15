using VvdKRepositry.Repositries.Contracts.Blob.Base;

namespace VvdKRepositry.Repositries.Contracts.Blob.General;

public interface IGeneralBlobPersistence : IBaseBlobPersistence
{
    static string StorageServiceIdentifier => "shared";
}