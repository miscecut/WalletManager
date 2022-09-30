using Microsoft.EntityFrameworkCore;
using Misce.WalletManager.Model.Models;

namespace Misce.WalletManager.Model.Data
{
    //Add-Migration migration_name
    //Update-Database applies the non-applied migrations
    //Remove-Migration removes the last non-applied migration
    public class WalletManagerContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<AccountType> AccountTypes { get; set; } = null!;
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<TransactionCategory> Categories { get; set; } = null!;
        public DbSet<TransactionSubCategory> SubCategories { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;

        public WalletManagerContext(DbContextOptions<WalletManagerContext> options) : base(options) { }
    }
}
