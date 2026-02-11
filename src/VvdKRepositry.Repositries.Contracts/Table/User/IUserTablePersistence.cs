using VvdKRepositry.Repositries.Contracts.Notifications;
using VvdKRepositry.Repositries.Contracts.Table.Base;

namespace VvdKRepositry.Repositries.Contracts.Table.User;

public interface IUserTablePersistence<TIdProvider> : ITableRepositry, IUserPersistenceCreationNotifications 
    where TIdProvider : class,ITableStorageParameterProvider;