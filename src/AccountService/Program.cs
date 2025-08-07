using AccountService.Configuration;
using AccountService.Features;
using AccountService.Infrastructure.Extensions;
using AccountService.Middleware;
using FluentValidation;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddLogging();

builder.Services.AddRepositories(builder.Configuration);
builder.Services.AddExternalServices(builder.Configuration);

builder.Services.AddAuthKeyCloakConfiguration(builder.Configuration);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblies(typeof(Program).Assembly));
builder.Services.AddAutoMapper(_ => { }, typeof(Program).Assembly);
builder.Services.AddCorsConfiguration(builder.Configuration);
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCorsConfiguration();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthConfiguration();
app.UseSwaggerConfiguration();

app.MapControllers();
app.MapGet("/", () => "AccountService");

app.UseHealthChecks("/health");
app.MapHealthChecks("/health");

app.Run();