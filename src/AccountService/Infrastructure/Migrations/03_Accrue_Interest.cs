using AccountService.Infrastructure.Common;
using FluentMigrator;

namespace AccountService.Infrastructure.Migrations;

[Migration(3)]
public class AccrueInterest : SqlMigration
{
    protected override string GetUpSql(IServiceProvider services) => @"
        create or replace procedure accrue_interest(account_ids uuid[])
        language plpgsql
        as $$
        declare
            account_id uuid;
            acc record;
            interest_amount numeric;
        begin
            foreach account_id in array account_ids loop
                select * into acc from accounts where id = account_id for update;

                if acc is null then
                    raise notice 'Account % not found', account_id;
                    continue;
                end if;

                if acc.interest_rate is null or acc.interest_rate = 0 then
                    continue;
                end if;

                interest_amount := acc.balance * (acc.interest_rate / 100);

                update accounts
                set balance = balance + interest_amount
                where id = account_id;

                insert into transactions (
                    id, account_id, type, counterparty_account_id, amount, currency, description, date
                )
                values (
                    gen_random_uuid(), account_id, 2, null, interest_amount, acc.currency, 'Начисление процентов', now()
                );
            end loop;
        end;
        $$;

        create extension if not exists pgcrypto;
    ";

    protected override string GetDownSql(IServiceProvider services) => @"
        drop procedure if exists accrue_interest(uuid[]);
    ";
}
