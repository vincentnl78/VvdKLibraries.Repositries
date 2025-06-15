using VvdKRepositry.Repositries.Contracts.Table.Base;

namespace VvdKRepositry.Repositries.Contracts.Table.General;

public interface IGeneralTablePersistence:IBaseTablePersistence
{
    static string StorageServiceIdentifier => "shared";
}