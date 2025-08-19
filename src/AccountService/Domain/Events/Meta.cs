// ReSharper disable UnusedAutoPropertyAccessor.Global получение свойств
namespace AccountService.Domain.Events;

public record Meta
{
    public string Version { get; init; } = "v1";
    public string Source { get; init; } = "account-service";
    public string CorrelationId { get; init; } = string.Empty;
    public string CausationId { get; init; } = string.Empty;
}