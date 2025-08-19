using AccountService.Configuration.Settings;
using Microsoft.Extensions.Options;

namespace AccountService.Configuration;

public static class CorsConfiguration
{
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        var settingsSection = configuration.GetSection(nameof(CorsOptions));
        services.Configure<CorsOptions>(settingsSection);
        var settings = settingsSection.Get<CorsOptions>()
                       ?? throw new NullReferenceException("Cors settings not found");

        services.AddCors(options =>
        {
            options.AddPolicy(settings.NamePolicy, policy =>
            {
                var origins = settings.AllowedOrigins
                    .Split(',', ';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                var hasAnyWildcard = origins.Length == 0 || origins.Any(o => o == "*" || o.Contains('*'));

                policy.AllowAnyHeader();
                policy.AllowAnyMethod();

                if (!hasAnyWildcard)
                {
                    policy.WithOrigins(origins)
                        .AllowCredentials();
                }
                else
                {
                    policy.AllowAnyOrigin();
                }

                policy.WithExposedHeaders("Content-Disposition");
            });
        });

        return services;
    }

    public static void UseCorsConfiguration(this IApplicationBuilder app)
    {
        var settings = app.ApplicationServices.GetRequiredService<IOptions<CorsOptions>>().Value;
        app.UseCors(settings.NamePolicy);
    }
}