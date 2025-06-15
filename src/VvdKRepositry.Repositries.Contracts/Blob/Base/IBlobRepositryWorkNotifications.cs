using MediatR;
using VvdKRepositry.Repositries.Contracts.Notifications.Repositry;

namespace VvdKRepositry.Repositries.Contracts.Blob.Base;

public interface IRepositryWorkNotifications :
    INotificationHandler<CommitChangesRepositryNotification>,
    INotificationHandler<UnloadRepositryNotification>;