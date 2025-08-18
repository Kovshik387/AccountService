using AccountService.Infrastructure.Common;
using FluentMigrator;

namespace AccountService.Infrastructure.Migrations;

[Migration(4)]
public class OutBox : SqlMigration
{
    protected override string GetUpSql(IServiceProvider services) => @"
    create table outbox_messages (
      id              uuid         primary key,
      occurred_at     timestamptz  not null default now(),
      routing_key     text         not null,
      data            jsonb        not null,
      attempt_count   int          not null default 0,
      next_attempt_at timestamptz  null,
      last_error      text         null
    );
";

    protected override string GetDownSql(IServiceProvider services) => @"
    drop table outbox_messages;
";
}