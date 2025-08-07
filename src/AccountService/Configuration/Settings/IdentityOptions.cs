namespace AccountService.Configuration.Settings;

public record IdentityOptions
{
    public required string Url { get; init; }
    public required string Realm { get; init; }
    public required string TypeAuthorization { get; init; }
}