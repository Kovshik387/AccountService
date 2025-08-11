namespace AccountService.Configuration.Settings;

public class HangFireOptions
{
    public required string CronExpression { get; init; }
}