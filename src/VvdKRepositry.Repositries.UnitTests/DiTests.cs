// using Azure.Data.Tables;
// using Azure.Storage.Blobs;
// using FluentAssertions;
// using MediatR;
// using Microsoft.Extensions.Azure;
// using Microsoft.Extensions.DependencyInjection;
// using VvdKRepositry.Repositries.Blob.User;
// using VvdKRepositry.Repositries.Contracts;
// using VvdKRepositry.Repositries.Contracts.Blob.User;
// using VvdKRepositry.Repositries.Contracts.Notifications.Repositry;
// using VvdKRepositry.Repositries.Contracts.Table.User;
// using VvdKRepositry.Repositries.Table.User;
// using Xunit.Abstractions;
//
// namespace VvdKRepositry.Repositries.UnitTests;
//
// public class DITests
// {
//     private readonly IServiceProvider _provider;
//
//     public DiTests(ITestOutputHelper testOutputHelper);
//     {
//         var services = new ServiceCollection();
//         var urls = new RegionalStorageUris(ApiAreas.E1, "test", "test2");
//
//         var configuration = new ConfigurationManager();
//         configuration.LoadAppConfigSettings();
//
//         services.TryAddScoped<IIdProvider>(_ => new IdProviderWithRegions(urls)
//         {
//             Id = "E1"
//         });
//         services.AddSingleton(TimeProvider.System);
//
//
//         services.AddCoreRepositries();
//         services.AddSingleton(JsonSerializerOptionsForUserAndCoreCreator.Create);
//         services.AddSingleton<IAzureClientFactory<BlobServiceClient>, FakeAzureClientFactory<BlobServiceClient>>();
//         services.AddSingleton<IAzureClientFactory<TableServiceClient>, FakeAzureClientFactory<TableServiceClient>>();
//
//
//         services.AddScoped<IUserBlobPersistence, UserBlobPersistence>();
//         services.AddSingleton<FakeBlobStore>();
//         services.AddSingleton<FakeTableStore>();
//         services.AddScoped<IUserTablePersistence, UserTablePersistence>();
//
//         services.ConfigureRepositry(configuration, ReplacementMode.Fake, new RepositryBackingOptions()
//         {
//             GeneralPersistence = true,
//             UserPersistence = true
//         });
//
//         _provider = services.BuildServiceProvider();
//         var idProvider = _provider.GetRequiredService<IIdProvider>();
//         idProvider.Id = "E1";
//     }
//
//     protected override bool LoadTestUser => false;
//
//     [Fact]
//     public void AccountRepositry()
//     {
//         var acc1 = _provider.GetRequiredService<IAccountsRepositry>();
//         var acc2 = _provider.GetRequiredService<IAccountsRepositry>();
//         acc1.Should().BeSameAs(acc2);
//
//         var ass1 = _provider.GetRequiredService<IAssetsRepositry>();
//         ass1.Should().BeSameAs(acc2);
//
//         var cat2 = _provider.GetRequiredService<ICategoriesRepositry>();
//         cat2.Should().BeSameAs(acc2);
//     }
//
//     [Fact]
//     public void AccountRepositryScoped()
//     {
//         var provider = _provider.CreateScope().ServiceProvider;
//
//         var acc1 = provider.GetRequiredService<IAccountsRepositry>();
//         var acc2 = provider.GetRequiredService<IAccountsRepositry>();
//         acc1.Should().BeSameAs(acc2);
//
//         var ass1 = provider.GetRequiredService<IAssetsRepositry>();
//         ass1.Should().BeSameAs(acc2);
//
//         var cat2 = provider.GetRequiredService<ICategoriesRepositry>();
//         cat2.Should().BeSameAs(acc2);
//     }
//
//     [Fact]
//     public void NotificationHandlerScoped()
//     {
//         var notificHandlers =
//             _provider.GetServices<INotificationHandler<CommitChangesRepositryNotification>>().ToList();
//
//         var repo = notificHandlers.First(t => t is AccountsRepositry) as AccountsRepositry;
//         repo!.Add(new Asset() { Name = "test" });
//
//         var notificHandlers2 = _provider.GetServices<INotificationHandler<CommitChangesRepositryNotification>>();
//         var repo2 = notificHandlers2.First(t => t is AccountsRepositry) as AccountsRepositry;
//
//         repo.Should().BeSameAs(repo2);
//         repo2!.All.Should().HaveCount(1);
//
//
//         notificHandlers.Should().NotBeEmpty();
//     }
//
//
//     [Fact]
//     public void ProofOfConcept()
//     {
//         var services = new ServiceCollection();
//         services.AddScoped<MyScopedService>();
//         services.AddTransient<INotificationHandler<MyNotification>, MyNotificationHandler>();
//         var provider = services.BuildServiceProvider();
//
//         var notificHandlers = provider.GetServices<INotificationHandler<MyNotification>>();
//         notificHandlers.Should().NotBeEmpty();
//
//         provider.GetServices<INotificationHandler<MyNotification>>();
//     }
//
//     [Fact]
//     public void NotificationHandler()
//     {
//         var idprovider = CoreTest.Provider.GetRequiredService<IIdProvider>();
//         idprovider.Id = "E1";
//         var notificHandlers = CoreTest.Provider.GetServices<INotificationHandler<CommitChangesRepositryNotification>>()
//             .ToList();
//
//         var repo = notificHandlers.First(t => t is AccountsRepositry) as AccountsRepositry;
//         repo!.Add(new Asset() { Name = "test" });
//
//         var notificHandlers2 =
//             CoreTest.Provider.GetServices<INotificationHandler<CommitChangesRepositryNotification>>();
//         var repo2 = notificHandlers2.First(t => t is AccountsRepositry) as AccountsRepositry;
//
//         repo.Should().BeSameAs(repo2);
//         repo2!.All.Should().HaveCount(1);
//         notificHandlers.Should().NotBeEmpty();
//     }
//
//     public class MyNotification : INotification;
//
//     public class MyScopedService
//     {
//     }
//
//     public class MyNotificationHandler() : INotificationHandler<MyNotification>
//     {
//         //private readonly MyScopedService _scopedService = scopedService;
//
//         public Task Handle(MyNotification notification, CancellationToken cancellationToken)
//         {
//             return Task.CompletedTask;
//         }
//     }
// }