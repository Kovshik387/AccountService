namespace AccountService.HealthCheck.Settings;

public class HealthOptions
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global Опция
    public int OutboxWarnThreshold { get; set; }
    // ReSharper disable once UnusedAutoPropertyAccessor.Global Опция
    public int RabbitMqTimeoutMs   { get; set; }
}