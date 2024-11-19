using Microsoft.EntityFrameworkCore;
using WalletEntities.Domain.Entities;

namespace WalletEntities.Data
{
    public class WalletDbContext : DbContext
    {
        public DbSet<AuthorizedUser> AuthorizedUsers { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options)
        {
        }
    }
}
