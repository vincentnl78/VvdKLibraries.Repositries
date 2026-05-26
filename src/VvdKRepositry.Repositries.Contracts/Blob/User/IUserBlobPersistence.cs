using VvdKRepositry.Repositries.Contracts.Blob.Base;
using VvdKRepositry.Repositries.Contracts.Notifications;

namespace VvdKRepositry.Repositries.Contracts.Blob.User;

//Implements IBlobRepositry, because container is already set!
public interface IUserBlobPersistence<TBlobStorageParameterProvider> : IBlobRepositry, IUserPersistenceCreationNotifications
    where TBlobStorageParameterProvider : class, IBlobStorageParameterProvider;