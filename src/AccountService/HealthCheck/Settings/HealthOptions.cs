namespace AccountService.HealthCheck.Settings;

public class HealthOptions
{
    public int OutboxWarnThreshold { get; set; }
    public int RabbitMqTimeoutMs   { get; set; }
}