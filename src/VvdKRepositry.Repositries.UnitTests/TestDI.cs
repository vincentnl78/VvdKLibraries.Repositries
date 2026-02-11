using System.Text.Json;
using AwesomeAssertions;
using Azure.Identity;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VvdKRepositry.Repositries.Contracts;
using VvdKRepositry.Repositries.Contracts.Notifications;
using VvdKRepositry.Repositries.Contracts.Notifications.Creation;
using VvdKRepositry.Repositries.Contracts.Notifications.Repositry;
using VvdKRepositry.Repositries.Contracts.Table.General;
using VvdKRepositry.Repositries.Table.General;

namespace VvdKRepositry.Repositries.UnitTests;

public class TestDependencyInjection
{
    public class TableStorageParameterProviderBasic : ITableStorageParameterProvider
    {
        public string Id { get; }
        public string? TableUri { get; set; }
        public string ServiceClientIdentifier { get; }
        public string TableName { get; }
    }
    
    public class BlobStorageParameterProviderBasic : IBlobStorageParameterProvider
    {
        public string Id { get; }
        public string BlobUri { get; }
        public string ServiceClientIdentifier { get; }
        public string BlobContainerName { get; }
    }
    
    [Fact]
    public void UserRepositryTesting()
    {
        //setup a scoped DI provider after adding services
        var services = new ServiceCollection();
        services.AddSingleton(new JsonSerializerOptions());
        
        services.AddTableStorageAndBlobStorageClients([
            new ConfigureServices.DynamicServiceProviderInfo()
            {
                BlobUri = "https://testbloburi.blob.core.windows.net/",
                TableUri = "https://testtableuri.table.core.windows.net/",
                ServiceIdentifier = "test-service-identifier"
            }
        ], new AzureCliCredential());
        services.AddUserIdProvider();
        services.AddUserBlobStoragePersistence<IBlobStorageParameterProvider,BlobStorageParameterProviderBasic>(ConfigureServices.NotificationHandlerTypes.ReadWriteCreateDelete);
        services.AddUserTableStoragePersistence<ITableStorageParameterProvider,TableStorageParameterProviderBasic>(ConfigureServices.NotificationHandlerTypes.ReadWriteCreateDelete);
        
        IServiceProvider provider = services.BuildServiceProvider();
        var result = provider.GetServices<INotificationHandler<CreateUserPersistenceSetupNotification>>().ToList();
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }
    
    [Fact]
    public void GeneralRepositryTesting()
    {
        var services = new ServiceCollection();
        services.AddSingleton(new JsonSerializerOptions());
        services.AddGeneralPersistence("https://testbloburi.blob.core.windows.net/","https://testtableuri.table.core.windows.net/", new AzureCliCredential());
        IServiceProvider provider = services.BuildServiceProvider();
        var result = provider.GetServices<IGeneralTablePersistence>().ToList();
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }


    public interface ITestGeneralRepo;
    public class TestGeneralRepo() : GeneralTableRepositry(new GeneralTablePersistence(null!)),ITestGeneralRepo
        , INotificationHandler<CommitChangesRepositryNotification>,INotificationHandler<UnloadRepositryNotification>
    {
        protected override string TableName { get; }
        public Task Handle(CommitChangesRepositryNotification notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task Handle(UnloadRepositryNotification notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
    
    [Theory]
    [InlineData(ConfigureServices.NotificationHandlerTypes.ReadOnly,1)]
    [InlineData(ConfigureServices.NotificationHandlerTypes.ReadWrite,2)]
    [InlineData(ConfigureServices.NotificationHandlerTypes.ReadWriteCreateDelete,4)]
    public void UserRepositryTestingNotifications(ConfigureServices.NotificationHandlerTypes type, int expectedCount)
    {
        var services = new ServiceCollection();
        //services.AddSingleton(new JsonSerializerOptions());
        services.AddWithNotifications<ITestGeneralRepo, TestGeneralRepo>(ServiceLifetime.Singleton,
            type);
        services.Count.Should().Be(expectedCount+1);
    }
}