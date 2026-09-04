using Auth.Interfaces;
using Auth.Interfaces.Storage;
using Auth.Objects;
using Auth.Repositories.Table;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using VvdKRepositry.Repositries;

namespace Auth.Repositories;

public static class ConfigureServices
{
    //add shared persistences (blob and table)
    //add concrete repositories (with notifications)

    public static void AddAuthPersistence(
        this IServiceCollection services, 
        string userTableUri,
        TokenCredential credential,
        VvdKRepositry.Repositries.ConfigureServices.NotificationHandlerTypes rights,
        PasswordRequirements passwordRequirements
    )
    {
        services.AddAuthPersistence(
            userTableUri,
            credential,
            passwordRequirements,
            rights
        );
    }

    private static void AddAuthPersistence(this IServiceCollection services, string userTableUri,
        TokenCredential credential,
        PasswordRequirements passwordRequirements
        ,VvdKRepositry.Repositries.ConfigureServices.NotificationHandlerTypes rights
    )
    {
        services.AddAuthServiceNoPersistence(passwordRequirements,rights);
        services.AddSingleton<IIdentityTablePersistence, IdentityTablePersistence>();
        services.AddAzureClients(builder =>
        {
            builder.AddTableServiceClient(new Uri(userTableUri))
                .WithCredential(credential)
                .WithName(IIdentityTablePersistence.StorageServiceIdentifier);
        });
    }

    //todo Document where this is needed? Unit Testing? better to remove manually and add the persistence in the test project?
    public static void AddAuthServiceNoPersistence(this IServiceCollection services,
        PasswordRequirements passwordRequirements,
        VvdKRepositry.Repositries.ConfigureServices.NotificationHandlerTypes rights)
    {
        services.AddIdentityCore<IdentityUser>(o =>
            {
                o.Password.RequireDigit = passwordRequirements.RequireDigit;
                o.Password.RequireLowercase = passwordRequirements.RequireLowercase;
                o.Password.RequireUppercase = passwordRequirements.RequireUppercase;
                o.Password.RequireNonAlphanumeric = passwordRequirements.RequireNonAlphanumeric;
                o.Password.RequiredLength = passwordRequirements.RequiredLength;

                o.User.RequireUniqueEmail = true;
                o.SignIn.RequireConfirmedEmail = true;
                o.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddDefaultTokenProviders();

        services.AddDataProtection();

        //Init Service
        services.AddSingleton<IUserStore<IdentityUser>, CustomUserStore>();

        //Repositries
        services.AddWithNotifications<IRefreshTokensTableRepositry, RefreshTokensGeneralTableRepositry>(ServiceLifetime.Singleton, rights);
        services.AddWithNotifications<IUserClaimsGeneralTableRepositry, UserClaimsGeneralGeneralTableRepositry>(ServiceLifetime.Singleton, rights);
        
        services.AddWithNotifications<IUsersGeneralTableRepositry, UsersGeneralTableRepositry>(ServiceLifetime.Singleton, rights);
        services.AddSingleton<IUsersGeneralTableReadOnlyRepositry>(s=> s.GetRequiredService<IUsersGeneralTableRepositry>());
    }
}