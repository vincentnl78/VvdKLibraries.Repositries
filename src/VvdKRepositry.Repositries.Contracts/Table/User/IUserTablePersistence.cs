using VvdKRepositry.Repositries.Contracts.Notifications;
using VvdKRepositry.Repositries.Contracts.Table.Base;

namespace VvdKRepositry.Repositries.Contracts.Table.User;

public interface IUserTablePersistence<TTableStorageParameterProvider> : ITableRepositry, IUserPersistenceCreationNotifications 
    where TTableStorageParameterProvider : class,ITableStorageParameterProvider;