using AccountService.Infrastructure.Common;
using FluentMigrator;

namespace AccountService.Infrastructure.Migrations;

[Migration(6)]
public class AccountBlock : SqlMigration
{
    protected override string GetUpSql(IServiceProvider services) => @"
    alter table accounts add column frozen boolean not null default false;
";

    protected override string GetDownSql(IServiceProvider services) => @"
    alter table accounts drop column if exists frozen;
";
}