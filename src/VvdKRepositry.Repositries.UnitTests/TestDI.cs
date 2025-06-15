using System.Text.Json;
using AwesomeAssertions;
using Azure.Identity;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VvdKRepositry.Repositries.Contracts.Notifications.Creation;
using VvdKRepositry.Repositries.Contracts.Table.General;

namespace VvdKRepositry.Repositries.UnitTests;

public class TestDependencyInjection
{
    [Fact]
    public void UserRepositryTesting()
    {
        //setup a scoped DI provider after adding services
        var services = new ServiceCollection();

        var idProvider = new IdProviderBasic
        {
            BlobUri = "https://testbloburi.blob.core.windows.net/",
            TableUri = "https://testtableuri.table.core.windows.net/"
        };
        services.AddSingleton(new JsonSerializerOptions());
        
        services.AddUserRepositryBacking(idProvider,new AzureCliCredential());
        
        IServiceProvider provider = services.BuildServiceProvider();
        var result = provider.GetServices<INotificationHandler<CreateUserPersistenceSetupNotification>>().ToList();
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }
    
    [Fact]
    public void GeneralRepositryTesting()
    {
        //setup a scoped DI provider after adding services
        var services = new ServiceCollection();

        var idProvider = new IdProviderBasic
        {
            BlobUri = "https://testbloburi.blob.core.windows.net/",
            TableUri = "https://testtableuri.table.core.windows.net/"
        };
        services.AddSingleton(new JsonSerializerOptions());
        
        services.AddGeneralRepositryBacking(idProvider.BlobUri,idProvider.TableUri, new AzureCliCredential());
        
        IServiceProvider provider = services.BuildServiceProvider();
        var result = provider.GetServices<IGeneralTablePersistence>().ToList();
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }
}