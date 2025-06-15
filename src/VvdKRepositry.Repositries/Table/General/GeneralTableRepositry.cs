using VvdKRepositry.Repositries.Contracts.Table.General;
using VvdKRepositry.Repositries.Table.Base;

namespace VvdKRepositry.Repositries.Table.General;

public abstract class GeneralTableRepositry(IGeneralTablePersistence persistence) : BaseTableRepositryWithCreationNotifiers(persistence);