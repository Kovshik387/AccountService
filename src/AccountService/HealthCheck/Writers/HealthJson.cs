using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AccountService.HealthCheck.Writers;

public static class HealthJson
{
    public static Task WriteHealthJson(HttpContext ctx, HealthReport report)
    {
        ctx.Response.ContentType = "application/json; charset=utf-8";
        var payload = new
        {
            status = report.Status.ToString(),
            entries = report.Entries.ToDictionary(
                e => e.Key,
                e => new {
                    status = e.Value.Status.ToString(),
                    desc   = e.Value.Description,
                    data   = e.Value.Data
                })
        };
        return ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }    
}