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
using VvdKRepositry.Repositries.Contracts.Table.General;
using VvdKRepositry.Repositries.Contracts.Table.User;
using VvdKRepositry.Repositries.Table.General;
using VvdKRepositry.Repositries.Table.User;

namespace VvdKRepositry.Repositries;

public static class ConfigureServices
{
    public static void AddGeneralRepositryBacking(this IServiceCollection services,
        string? sharedBlobUri,
        string? sharedTableUri,
        TokenCredential credential)
    {
        services.AddWithNotifications<IGeneralBlobPersistence, GeneralBlobPersistence>(ServiceLifetime.Singleton);
        services.AddWithNotifications<IGeneralTablePersistence, GeneralTablePersistence>(ServiceLifetime.Singleton);

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

    public static void AddUserRepositryBacking(
        this IServiceCollection services,
        IIdProvider idProvider,
        TokenCredential credential
    )
    {
        services.AddAzureClients(cb =>
        {
            services.AddWithNotifications<IUserBlobPersistence, UserBlobPersistence>(ServiceLifetime.Scoped);
            cb.AddBlobServiceClient(new Uri(idProvider.BlobUri))
                .WithName(idProvider.ServiceClientIdentifier)
                .WithCredential(credential);

            services.AddWithNotifications<IUserTablePersistence, UserTablePersistence>(ServiceLifetime.Scoped);
            cb.AddTableServiceClient(new Uri(idProvider.TableUri))
                .WithName(idProvider.ServiceClientIdentifier)
                .WithCredential(credential);
        });
        services.TryAddScoped<IIdProvider>(_ => idProvider);
    }

    public static void AddUserRepositryDynamicBacking<T>(this IServiceCollection services,
        List<DynamicServiceProviderInfo> dynamicServiceProviderInfos,
        TokenCredential credential
    )
        where T : class, IIdProvider
    {
        services.AddWithNotifications<IUserBlobPersistence, UserBlobPersistence>(ServiceLifetime.Scoped);
        services.AddWithNotifications<IUserTablePersistence, UserTablePersistence>(ServiceLifetime.Scoped);

        services.AddAzureClients(cb =>
        {
            foreach (var info in dynamicServiceProviderInfos)
            {
                cb.AddBlobServiceClient(new Uri(info.BlobUri))
                    .WithName(info.ServiceIdentifier)
                    .WithCredential(credential);

                cb.AddTableServiceClient(new Uri(info.TableUri))
                    .WithName(info.ServiceIdentifier)
                    .WithCredential(credential);
            }
        });
        services.TryAddScoped<IIdProvider, T>();
    }

    public struct DynamicServiceProviderInfo
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public string BlobUri { get; set; }

        public string TableUri { get; set; }

        public string ServiceIdentifier { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
    }

    #region Service Removal

    public static bool IsNotificationHandler(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(INotificationHandler<>);
    }

    public static void AddWithNotifications<TInterface, TImplementation>(this IServiceCollection services,
        ServiceLifetime lifetime)
        where TImplementation : class, TInterface
        where TInterface : class
    {
        if (services.All(s => s.ServiceType != typeof(TInterface)))
            services.Scan(s => s
                .FromType<TImplementation>()
                .AsSelfWithInterfaces(f => IsNotificationHandler(f) || f == typeof(TInterface))
                .WithLifetime(lifetime)
            );
    }

    #endregion
}