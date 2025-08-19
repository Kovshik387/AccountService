using AccountService.Configuration.Settings;
using Serilog;
using Serilog.Events;

namespace AccountService.Configuration;

public static class LoggerConfiguration
{
    public static void AddAppLogger(this WebApplicationBuilder builder)
    {
        var settings = builder.Configuration.GetSection(nameof(LoggerOptions)).Get<LoggerOptions>();
        
        if (settings is null) throw new NullReferenceException("Logger settings not found");
        
        var loggerConfiguration = new Serilog.LoggerConfiguration();

        loggerConfiguration
            .Enrich.WithCorrelationIdHeader()
            .Enrich.FromLogContext();

        if (!Enum.TryParse(settings.Level, out LogLevels level)) level = LogLevels.Information;

        var serilogLevel = level switch
        {
            LogLevels.Verbose => LogEventLevel.Verbose,
            LogLevels.Debug => LogEventLevel.Debug,
            LogLevels.Information => LogEventLevel.Information,
            LogLevels.Warning => LogEventLevel.Warning,
            LogLevels.Error => LogEventLevel.Error,
            LogLevels.Fatal => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };

        loggerConfiguration
            .MinimumLevel.Is(serilogLevel)
            .MinimumLevel.Override("Microsoft", serilogLevel)
            .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", serilogLevel)
            .MinimumLevel.Override("System", serilogLevel)
            ;

        var logItemTemplate =
            "[{Timestamp:HH:mm:ss:fff} {Level:u3} ({CorrelationId})] {Message:lj}{NewLine}{Exception}";

        if (settings.WriteToConsole)
            loggerConfiguration.WriteTo.Console(
                serilogLevel,
                logItemTemplate
            );

        if (settings.WriteToFile)
        {
            if (!Enum.TryParse(settings.FileRollingInterval, out LogRollingInterval interval))
                interval = LogRollingInterval.Day;

            var serilogInterval = interval switch
            {
                LogRollingInterval.Infinite => RollingInterval.Infinite,
                LogRollingInterval.Year => RollingInterval.Year,
                LogRollingInterval.Month => RollingInterval.Month,
                LogRollingInterval.Day => RollingInterval.Day,
                LogRollingInterval.Hour => RollingInterval.Hour,
                LogRollingInterval.Minute => RollingInterval.Minute,
                _ => RollingInterval.Day
            };


            if (!int.TryParse(settings.FileRollingSize, out var size)) size = 5242880;

            var fileName = $"_.log";

            loggerConfiguration.WriteTo.File($"logs/{fileName}",
                serilogLevel,
                logItemTemplate,
                rollingInterval: serilogInterval,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: size
            );
        }

        var logger = loggerConfiguration.CreateLogger();

        builder.Host.UseSerilog(logger, true);
    }
}