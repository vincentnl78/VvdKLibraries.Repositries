using Auth.Interfaces;
using Auth.Objects;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Service;

public static class ConfigureServices
{
    
    public static void AddAuthenticationServicesAsAuthenticator(this IServiceCollection services, JwtAuthenticationSettings jwtAuthenticationSettings)
    {
        services.AddSingleton(jwtAuthenticationSettings);
        services.AddScoped<ITokenValidationService, TokenValidationService>();
    }
    
    public static void AddAuthenticationServicesAsLifeCycleManagerAndTokenProvider(this IServiceCollection services, 
        JwtAuthenticationSettings jwtAuthenticationSettings,
        RegionalBackendUrlConnectionStrings connectionStrings,
        PasswordRequirements passwordRequirements
        )
    {
        services.AddAuthenticationServicesAsAuthenticator(jwtAuthenticationSettings);
        services.AddScoped<IUserValidationService, UserValidationService>();
        services.AddSingleton(connectionStrings);
        services.AddSingleton(passwordRequirements);
        services.AddScoped<IUserReadService, UserReadService>(); //scoped, ingests scoped UserManager
        services.AddScoped<IDatabaseUserTokenGenerationService, DatabaseUserTokenGenerationService>(); //scoped, ingests scoped UserManager
        services.AddScoped<ITokenGenerationSerivce, TokenGenerationService>();
        services.AddScoped<IDatabaseUserUpdateService, UserUpdateService>(); //scoped, ingests scoped UserManager
    }
    
    public static void AddAuthenticationServicesAsAdmin(this IServiceCollection services)
    {
        services.AddScoped<IUserReadService, UserReadService>(); //scoped, ingests scoped UserManager
        services.AddScoped<IDatabaseUserUpdateService, UserUpdateService>(); //scoped, ingests scoped UserManager
    }
}