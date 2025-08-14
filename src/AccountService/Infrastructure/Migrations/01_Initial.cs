using AccountService.Infrastructure.Common;
using FluentMigrator;

namespace AccountService.Infrastructure.Migrations;
[Migration(1)]
public class Initial : SqlMigration
{

    protected override string GetUpSql(IServiceProvider services) => @"
        create table accounts (
            id              uuid primary key,
            owner_id        uuid not null,
            type            int not null,
            currency        text not null,
            balance         numeric not null,
            interest_rate   numeric,
            opening_date    timestamp with time zone not null,
            closing_date    timestamp with time zone
    );

        create table transactions (
            id                      uuid primary key,
            account_id              uuid not null references accounts(id) on delete cascade,
            type                    int not null,
            counterparty_account_id uuid,
            amount                  numeric not null,
            currency                text not null,
            description             text not null,
            date                    timestamp with time zone not null
    );
";

    protected override string GetDownSql(IServiceProvider services) => @"
        drop table accounts;
        drop table transactions;
";
}