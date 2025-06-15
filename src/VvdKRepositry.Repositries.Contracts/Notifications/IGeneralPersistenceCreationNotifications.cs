using MediatR;
using VvdKRepositry.Repositries.Contracts.Notifications.Creation;

namespace VvdKRepositry.Repositries.Contracts.Notifications;

public interface IGeneralPersistenceCreationNotifications :
    INotificationHandler<CreateGeneralPersistenceSetupNotification>,
    INotificationHandler<DeleteGeneralPersistenceNotification>;