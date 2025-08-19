using AccountService.Domain.Events;
using AccountService.Domain.Events.Publish;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AccountService.Configuration.Filter;

public class EventsFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var rel = context.ApiDescription.RelativePath ?? string.Empty;
        if (!rel.Equals("api/transactions/transfer", StringComparison.OrdinalIgnoreCase))
            return;
        
        var shell = typeof(EventShell<>);
        var accountOpened   = context.SchemaGenerator.GenerateSchema(shell.MakeGenericType(typeof(AccountOpened)),   context.SchemaRepository);
        var moneyCredited   = context.SchemaGenerator.GenerateSchema(shell.MakeGenericType(typeof(MoneyCredited)),   context.SchemaRepository);
        var moneyDebited    = context.SchemaGenerator.GenerateSchema(shell.MakeGenericType(typeof(MoneyDebited)),    context.SchemaRepository);
        var transferDone    = context.SchemaGenerator.GenerateSchema(shell.MakeGenericType(typeof(TransferCompleted)),context.SchemaRepository);

        var unionSchema = new OpenApiSchema
        {
            OneOf = new List<OpenApiSchema> { accountOpened, moneyCredited, moneyDebited, transferDone }
        };

        var pathItem = new OpenApiPathItem();
        pathItem.AddOperation(OperationType.Post, new OpenApiOperation
        {
            Summary = "События",
            RequestBody = new OpenApiRequestBody
            {
                Required = true,
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new() { Schema = unionSchema }
                }
            },
            Responses = new OpenApiResponses { ["200"] = new OpenApiResponse { Description = "OK" } }
        });

        operation.Callbacks ??= new Dictionary<string, OpenApiCallback>();
        var callback = new OpenApiCallback();
        callback.AddPathItem(RuntimeExpression.Build("$request.body#/callbackUrl"), pathItem);
        operation.Callbacks["accountEvents"] = callback;
    }
}
