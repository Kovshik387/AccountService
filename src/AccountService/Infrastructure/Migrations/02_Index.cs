using AccountService.Infrastructure.Common;
using FluentMigrator;

namespace AccountService.Infrastructure.Migrations;

[Migration(2, TransactionBehavior.None)]
public class Index : SqlMigration
{
    protected override string GetUpSql(IServiceProvider services) => @"
        create extension if not exists btree_gist;

        create index if not exists idx_accounts_owner_id_hash on accounts using hash (owner_id);

        create index if not exists idx_transactions_account_date on transactions (account_id, date);

        create index if not exists idx_transactions_date_gist on transactions using gist (date);
    ";

    protected override string GetDownSql(IServiceProvider services) => @"
        drop index if exists idx_transactions_date_gist;
        drop index if exists idx_transactions_account_date;
        drop index if exists idx_accounts_owner_id_hash;

        drop extension if exists btree_gist;
    ";
}