using AccountService.Infrastructure.Common;
using FluentMigrator;

namespace AccountService.Infrastructure.Migrations;

[Migration(5)]
public class InboxConsumed : SqlMigration
{
    protected override string GetUpSql(IServiceProvider services) => @"
create table if not exists inbox_consumed
(
    message_id   uuid        primary key,
    processed_at timestamptz not null default now(),
    handler      text        not null
);
";
    protected override string GetDownSql(IServiceProvider services) => @"drop table if exists inbox_consumed;";
}