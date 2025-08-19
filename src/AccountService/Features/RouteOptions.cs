namespace AccountService.Features;

public record RouteOptions
{
    public string AccountOpenEventRoute { get; init; } = string.Empty;
    public string DebitEventRoute { get; init; } = string.Empty;
    public string CreditEventRoute { get; init; } = string.Empty;
    public string TransferEventRoute { get; init; } = string.Empty;
    public string ClientBlockEventRoute { get; init; } = string.Empty;
    public string ClientUnblockEventRoute { get; init; } = string.Empty;
    public string AccrueEventRoute { get; init; } = string.Empty;
}