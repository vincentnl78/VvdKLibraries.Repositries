using VvdKRepositry.Repositries.Contracts.Table.Base;

namespace Auth.Interfaces.Storage;

public interface IIdentityTablePersistence : IBaseTablePersistence
{
    static string StorageServiceIdentifier => "IdentityTablePersistence";
}