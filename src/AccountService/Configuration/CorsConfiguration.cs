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
        var settings = settingsSection.Get<CorsOptions>();

        if (settings == null)
            throw new NullReferenceException("Cors settings not found");

        services.AddCors(options =>
        {
            options.AddPolicy(settings.NamePolicy, policy =>
            {
                var origins = settings.AllowedOrigins.Split(',', ';')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToArray();

                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowCredentials();

                if (origins.Length > 0 && !origins.Contains("*"))
                    policy.WithOrigins(origins);
                else
                    policy.SetIsOriginAllowed(_ => true);

                policy.WithExposedHeaders("Content-Disposition");
            });
        });

        return services;
    }

    public static void UseCorsConfiguration(this IApplicationBuilder app)
    {
        var settings = app.ApplicationServices
            .GetRequiredService<IOptions<CorsOptions>>().Value;

        app.UseCors(settings.NamePolicy);
    }
}