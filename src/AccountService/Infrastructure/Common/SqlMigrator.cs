using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;

namespace AccountService.Infrastructure.Common;

public abstract class SqlMigration : IMigration
{
    public void GetUpExpressions(IMigrationContext context)
    {
        _ = context ?? throw new ArgumentNullException(nameof(context));

        context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = GetUpSql(context.ServiceProvider) });
    }
    
    public void GetDownExpressions(IMigrationContext context)
    {
        _ = context ?? throw new ArgumentNullException(nameof(context));

        context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = GetDownSql(context.ServiceProvider) });
    }

    protected abstract string GetUpSql(IServiceProvider services);
    protected abstract string GetDownSql(IServiceProvider services);

    string IMigration.ConnectionString => throw new NotImplementedException();
}