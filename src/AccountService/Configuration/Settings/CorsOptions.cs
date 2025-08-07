namespace AccountService.Configuration.Settings;

public record CorsOptions
{
    public required string AllowedOrigins { get; init; }
    public required string NamePolicy { get; init; }
}