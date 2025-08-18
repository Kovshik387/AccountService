namespace AccountService.Infrastructure.Settings;

public record RabbitMqOptions
{
    public required string Host { get; init; }
    public int Port { get; init; }
    public required string UserName { get; init; }
    public required string Password { get; init; }
    public required string VHost { get; init; }
}