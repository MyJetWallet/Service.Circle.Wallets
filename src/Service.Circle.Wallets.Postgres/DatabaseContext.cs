using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Postgres;
using MyJetWallet.Sdk.Service;
using Service.Circle.Wallets.Postgres.Models;

namespace Service.Circle.Wallets.Postgres
{
    public class DatabaseContext : MyDbContext
    {
        public const string Schema = "circle";

        private const string CardsTableName = "cards";

        private const string BankAccountsTableName = "bank_accounts";

        private Activity _activity;

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<CircleCardEntity> Cards { get; set; }

        public DbSet<CircleBankAccountEntity> BankAccounts { get; set; }

        public static DatabaseContext Create(DbContextOptionsBuilder<DatabaseContext> options)
        {
            var activity = MyTelemetry.StartActivity($"Database context {Schema}")?.AddTag("db-schema", Schema);

            var ctx = new DatabaseContext(options.Options) { _activity = activity };

            return ctx;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            SetCardEntry(modelBuilder);
            SetBankAccountEntry(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SetBankAccountEntry(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CircleBankAccountEntity>().ToTable(BankAccountsTableName);
            modelBuilder.Entity<CircleBankAccountEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<CircleBankAccountEntity>().Property(e => e.BrokerId).HasMaxLength(128);
            modelBuilder.Entity<CircleBankAccountEntity>().Property(e => e.ClientId).HasMaxLength(128);
            modelBuilder.Entity<CircleBankAccountEntity>().Property(e => e.Error).HasMaxLength(2048).IsRequired(false);
            modelBuilder.Entity<CircleBankAccountEntity>().Property(e => e.IsActive);
            modelBuilder.Entity<CircleBankAccountEntity>().Property(e => e.CreateDate);
            modelBuilder.Entity<CircleBankAccountEntity>().Property(e => e.UpdateDate);

            modelBuilder.Entity<CircleBankAccountEntity>().HasIndex(e => new { e.BrokerId, e.ClientId });
            modelBuilder.Entity<CircleBankAccountEntity>().HasIndex(e => new { e.BrokerId, e.ClientId, e.IsActive });
            modelBuilder.Entity<CircleBankAccountEntity>().HasIndex(e => new { e.BankAccountId });
        }

        private void SetCardEntry(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CircleCardEntity>().ToTable(CardsTableName);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.Id);
            modelBuilder.Entity<CircleCardEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<CircleCardEntity>().HasIndex(e => e.Id).IsUnique();
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.BrokerId).HasMaxLength(128);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.ClientId).HasMaxLength(128);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.CardName).HasMaxLength(128);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.CircleCardId).HasMaxLength(128).IsRequired(false);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.ExpMonth).IsRequired(false);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.ExpYear).IsRequired(false);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.Status);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.Network).HasMaxLength(128).IsRequired(false);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.Last4).HasMaxLength(64).IsRequired(false);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.Error).HasMaxLength(2048).IsRequired(false);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.IsActive);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.CreateDate);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.UpdateDate);

            modelBuilder.Entity<CircleCardEntity>().HasIndex(e => new { e.BrokerId, e.ClientId });
            modelBuilder.Entity<CircleCardEntity>().HasIndex(e => new { e.BrokerId, e.ClientId, e.IsActive });
            modelBuilder.Entity<CircleCardEntity>().HasIndex(e => e.CircleCardId).IsUnique();
        }

        public async Task<int> InsertAsync(CircleCardEntity entity)
        {
            var result = await Cards.Upsert(entity).On(e => e.Id).NoUpdate().RunAsync();
            return result;
        }

        public async Task UpdateAsync(CircleCardEntity entity)
        {
            await UpdateAsync(new List<CircleCardEntity> { entity });
        }

        public async Task UpdateAsync(CircleBankAccountEntity entity)
        {
            await UpdateAsync(new List<CircleBankAccountEntity> { entity });
        }

        public async Task UpdateAsync(IEnumerable<CircleBankAccountEntity> entities)
        {
            BankAccounts.UpdateRange(entities);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(IEnumerable<CircleCardEntity> entities)
        {
            Cards.UpdateRange(entities);
            await SaveChangesAsync();
        }
    }
}