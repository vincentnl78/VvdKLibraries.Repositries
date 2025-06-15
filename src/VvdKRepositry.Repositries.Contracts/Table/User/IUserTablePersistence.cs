using VvdKRepositry.Repositries.Contracts.Notifications;
using VvdKRepositry.Repositries.Contracts.Table.Base;

namespace VvdKRepositry.Repositries.Contracts.Table.User;

public interface IUserTablePersistence : ITableRepositry, IUserPersistenceCreationNotifications;
    
