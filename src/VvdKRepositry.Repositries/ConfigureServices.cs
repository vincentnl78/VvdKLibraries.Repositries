using Azure.Core;
using MediatR;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scrutor;
using VvdKRepositry.Repositries.Blob.General;
using VvdKRepositry.Repositries.Blob.User;
using VvdKRepositry.Repositries.Contracts;
using VvdKRepositry.Repositries.Contracts.Blob.General;
using VvdKRepositry.Repositries.Contracts.Blob.User;
using VvdKRepositry.Repositries.Contracts.Notifications.Creation;
using VvdKRepositry.Repositries.Contracts.Notifications.Repositry;
using VvdKRepositry.Repositries.Contracts.Table.General;
using VvdKRepositry.Repositries.Contracts.Table.User;
using VvdKRepositry.Repositries.Table.General;
using VvdKRepositry.Repositries.Table.User;

namespace VvdKRepositry.Repositries;

public static class ConfigureServices
{
    #region General Repositry Backing

    public static void AddGeneralPersistence(this IServiceCollection services,
        string? sharedBlobUri,
        string? sharedTableUri,
        TokenCredential credential)
    {
        //persistence does not have notifications, only repositories!
        services.AddSingleton<IGeneralBlobPersistence, GeneralBlobPersistence>();
        services.AddSingleton<IGeneralTablePersistence, GeneralTablePersistence>();
        
        /*services.AddWithNotifications<IGeneralBlobPersistence, GeneralBlobPersistence>(ServiceLifetime.Singleton,write
            ? NotificationHandlerTypes.Commit | NotificationHandlerTypes.Unload | NotificationHandlerTypes.CreateGeneral | NotificationHandlerTypes.DeleteGeneral
            : NotificationHandlerTypes.Unload
            );
        services.AddWithNotifications<IGeneralTablePersistence, GeneralTablePersistence>(ServiceLifetime.Singleton,write
            ? NotificationHandlerTypes.Commit | NotificationHandlerTypes.Unload | NotificationHandlerTypes.CreateGeneral | NotificationHandlerTypes.DeleteGeneral
            : NotificationHandlerTypes.Unload);*/

        services.AddAzureClients(cb =>
        {
            if (sharedBlobUri is not null)
                cb.AddBlobServiceClient(new Uri(sharedBlobUri))
                    .WithName(IGeneralBlobPersistence.StorageServiceIdentifier)
                    .WithCredential(credential);

            if (sharedTableUri is not null)
                cb.AddTableServiceClient(new Uri(sharedTableUri))
                    .WithName(IGeneralTablePersistence.StorageServiceIdentifier)
                    .WithCredential(credential);
        });
    }

    #endregion

    #region Id dependent Repositry Backing
    public static void AddUserIdProvider(this IServiceCollection services) 
    {
        services.AddScoped<IUserIdProvider, UserIdProvider>();
    }
    
    public static void AddUserIdProvider<TUserIdProvider>(this IServiceCollection services)
        where TUserIdProvider : class, IUserIdProvider
    {
        services.AddScoped<IUserIdProvider, TUserIdProvider>();
    }
    
    
    public static void AddTableStorageAndBlobStorageClients(
        this IServiceCollection services,
        List<DynamicServiceProviderInfo> serviceProviderInfo,
        TokenCredential credential
    )
    {
        services.AddAzureClients(cb =>
        {
            foreach (var info in serviceProviderInfo)
            {
                if(info.BlobUri!=null)
                    cb.AddBlobServiceClient(new Uri(info.BlobUri))
                        .WithName(info.ServiceIdentifier)
                        .WithCredential(credential);
                if(info.TableUri!=null)
                    cb.AddTableServiceClient(new Uri(info.TableUri))
                        .WithName(info.ServiceIdentifier)
                        .WithCredential(credential);
            }
        });
    }
    
    
    public static void AddUserTableStoragePersistence<TStorageParameterInterface, TStorageParameterImplementation>(
        this IServiceCollection services, NotificationHandlerTypes rights
    )
        where TStorageParameterInterface : class, ITableStorageParameterProvider
        where TStorageParameterImplementation : class, TStorageParameterInterface
    {
        services.AddWithNotifications<IUserTablePersistence<TStorageParameterInterface>, UserTablePersistence<TStorageParameterInterface>>(ServiceLifetime.Scoped,rights);
        services.TryAddScoped<TStorageParameterInterface,TStorageParameterImplementation>();
    }
    
    public static void AddUserBlobStoragePersistence<TStorageParameterInterface, TStorageParameterImplementation>(
        this IServiceCollection services, NotificationHandlerTypes rights
    )
        where TStorageParameterInterface : class, IBlobStorageParameterProvider
        where TStorageParameterImplementation : class,TStorageParameterInterface
    {
        services.AddWithNotifications<IUserBlobPersistence<TStorageParameterInterface>, UserBlobPersistence<TStorageParameterInterface>>(ServiceLifetime.Scoped,rights);
        services.TryAddScoped<TStorageParameterInterface,TStorageParameterImplementation>();
    }

    public struct DynamicServiceProviderInfo
    {
        public string? BlobUri { get; set; }

        public string? TableUri { get; set; }
        public string ServiceIdentifier { get; set; }
    }
    #endregion

    #region Service Removal

    
    [Flags]
    public enum NotificationHandlerTypes
    {
        ReadOnly=1,
        ReadWrite=ReadOnly| 2,
        ReadWriteCreateDelete= ReadWrite| 4
    }

    
    public static bool IsNotificationHandler(Type type, NotificationHandlerTypes typesToRegister)
    {
        
        if (!type.IsGenericType)
            return false;
        switch (type)
        {
            //when type is INotificationHandler<CommitChangesRepositryNotification>
            case var t when t == typeof(INotificationHandler<CommitChangesRepositryNotification>):
                 return typesToRegister.HasFlag(NotificationHandlerTypes.ReadWrite);
            case var t when t == typeof(INotificationHandler<UnloadRepositryNotification>):
                return true;
            case var t when t == typeof(INotificationHandler<CreateGeneralPersistenceSetupNotification>):
                return typesToRegister.HasFlag(NotificationHandlerTypes.ReadWriteCreateDelete);
            case var t when t == typeof(INotificationHandler<DeleteGeneralPersistenceNotification>):
                return typesToRegister.HasFlag(NotificationHandlerTypes.ReadWriteCreateDelete);
            case var t when t == typeof(INotificationHandler<CreateUserPersistenceSetupNotification>):
                return typesToRegister.HasFlag(NotificationHandlerTypes.ReadWriteCreateDelete);
            case var t when t == typeof(INotificationHandler<DeleteUserPersistenceNotification>):
                return typesToRegister.HasFlag(NotificationHandlerTypes.ReadWriteCreateDelete);
            default:
                return false;
        }
        //types
        // CommitChangesRepositryNotification
        // UnloadRepositryNotification
        
        // CreateGeneralPersistenceSetupNotification
        // DeleteGeneralPersistenceNotification
        
        // CreateUserPersistenceSetupNotification
        // DeleteUserPersistenceNotification
        
        
        //return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(INotificationHandler<>);
    }

    public static void AddWithNotifications<TInterface, TImplementation>(this IServiceCollection services,
        ServiceLifetime lifetime,NotificationHandlerTypes typesToRegister)
        where TImplementation : class, TInterface
        where TInterface : class
    {
        if (services.All(s => s.ServiceType != typeof(TInterface)))
            services.Scan(s => s
                .FromType<TImplementation>()
                //.AsImplementedInterfaces(f => IsNotificationHandler(f,typesToRegister) || f == typeof(TInterface))
                .AsSelfWithInterfaces(f => IsNotificationHandler(f,typesToRegister) || f == typeof(TInterface))
                .WithLifetime(lifetime)
            );
    }
    #endregion
}