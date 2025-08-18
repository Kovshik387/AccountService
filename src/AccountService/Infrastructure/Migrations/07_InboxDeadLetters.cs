using AccountService.Infrastructure.Common;
using FluentMigrator;

namespace AccountService.Infrastructure.Migrations;

[Migration(7)]
public class InboxDeadLetters : SqlMigration
{
    protected override string GetUpSql(IServiceProvider services) => @"
create table if not exists inbox_dead_letters
(
    message_id  uuid        not null,
    received_at timestamptz not null default now(),
    handler     text        not null,
    payload     jsonb       not null,
    error       text        not null
);

create index if not exists ix_inbox_dead_letters_received_at on inbox_dead_letters (received_at desc);
";
    protected override string GetDownSql(IServiceProvider services) => @"
drop table if exists inbox_dead_letters;
drop index if exists ix_inbox_dead_letters_received_at;
";
}