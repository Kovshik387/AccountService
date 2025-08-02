using AccountService.Configuration.Settings;
using Microsoft.IdentityModel.Tokens;

namespace AccountService.Configuration;

public static class AuthKeyCloakConfiguration
{
    public static IServiceCollection AddAuthKeyCloakConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var settingsSection = configuration.GetSection(nameof(IdentityOptions));
        services.Configure<IdentityOptions>(settingsSection);
        var settings = settingsSection.Get<IdentityOptions>();

        if (settings == null)
            throw new NullReferenceException("Identity settings not found");
        
        services.AddAuthentication(settings.TypeAuthorization)
            .AddJwtBearer(settings.TypeAuthorization, options =>
            {
                options.Authority = $"http://keycloak:8080/realms/{settings.Realm}";
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    ValidIssuer = $"{settings.Url}/realms/{settings.Realm}"
                };

            });

        services.AddAuthorization();
        
        return services;
    }

    public static void UseAuthConfiguration(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}