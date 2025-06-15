using MediatR;
using VvdKRepositry.Repositries.Contracts.Notifications.Creation;

namespace VvdKRepositry.Repositries.Contracts.Notifications;

public interface IUserPersistenceCreationNotifications :
    INotificationHandler<CreateUserPersistenceSetupNotification>,
    INotificationHandler<DeleteUserPersistenceNotification>;