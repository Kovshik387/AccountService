using AccountService.Configuration;
using AccountService.Features;
using AccountService.Features.Outbox.Extensions;
using AccountService.HealthCheck;
using AccountService.HealthCheck.Writers;
using AccountService.Infrastructure.Extensions;
using AccountService.Middleware;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddHealthChecks()
    .AddCheck("live", () => HealthCheckResult.Healthy("OK"), tags: ["live"])
    .AddCheck<RabbitMqHealthCheck>("rabbitmq", tags: ["ready"])
    .AddCheck<OutboxHealthCheck>("outbox", tags: ["ready"]);
builder.Services.AddRepositories(builder.Configuration);
builder.Services.AddExternalServices(builder.Configuration);
builder.Services.AddMessageBus(builder.Configuration);

builder.Services.AddAuthKeyCloakConfiguration(builder.Configuration);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblies(typeof(Program).Assembly));
builder.Services.AddAutoMapper(_ => { }, typeof(Program).Assembly);
builder.AddAppLogger();
builder.Services.AddCorsConfiguration(builder.Configuration);
builder.Services.AddSwaggerConfiguration();

builder.Services.AddJobs();
builder.Services.AddHangFire(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCorsConfiguration();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthConfiguration();
app.UseSwaggerConfiguration();
app.UseHangFireConfiguration();
app.MapControllers();
app.MapGet("/", () => "AccountService");

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("live"),
    ResponseWriter = HealthJson.WriteHealthJson
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("ready"),
    ResponseWriter = HealthJson.WriteHealthJson
});
app.MigrateUp();

app.Run();

public partial class Program
{
}