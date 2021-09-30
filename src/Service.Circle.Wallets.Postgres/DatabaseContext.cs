using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using Service.Circle.Wallets.Postgres.Models;

namespace Service.Circle.Wallets.Postgres
{
    public class DatabaseContext : DbContext
    {
        public const string Schema = "circle";

        private const string TransfersTableName = "cards";

        private Activity _activity;

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<CircleCardEntity> Cards { get; set; }
        public static ILoggerFactory LoggerFactory { get; set; }

        public static DatabaseContext Create(DbContextOptionsBuilder<DatabaseContext> options)
        {
            var activity = MyTelemetry.StartActivity($"Database context {Schema}")?.AddTag("db-schema", Schema);

            var ctx = new DatabaseContext(options.Options) { _activity = activity };

            return ctx;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (LoggerFactory != null) optionsBuilder.UseLoggerFactory(LoggerFactory).EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            SetCardEntry(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SetCardEntry(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CircleCardEntity>().ToTable(TransfersTableName);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.Id).UseIdentityColumn();
            modelBuilder.Entity<CircleCardEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.BrokerId).HasMaxLength(128);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.ClientId).HasMaxLength(128);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.CircleCardId).HasMaxLength(128);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.BillingName).HasMaxLength(512);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.BillingCity).HasMaxLength(512);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.BillingCountry).HasMaxLength(128);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.BillingLine1).HasMaxLength(1024);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.BillingLine2).HasMaxLength(1024).IsRequired(false);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.BillingDistrict).HasMaxLength(1024);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.BillingPostalCode).HasMaxLength(128);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.ExpMonth);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.ExpYear);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.Email).HasMaxLength(1024);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.PhoneNumber).HasMaxLength(128);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.SessionId).HasMaxLength(256).IsRequired(false);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.IpAddress).HasMaxLength(256).IsRequired(false);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.Status);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.Network).HasMaxLength(128);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.Last4).HasMaxLength(64);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.Bin).HasMaxLength(64);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.IssuerCountry).HasMaxLength(128);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.FundingType);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.Fingerprint).HasMaxLength(1024);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.ErrorCode).IsRequired(false);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.CreateDate).IsRequired(false);
            modelBuilder.Entity<CircleCardEntity>().Property(e => e.UpdateDate).IsRequired(false);
            
            modelBuilder.Entity<CircleCardEntity>().HasIndex(e => new {e.BrokerId, e.ClientId});
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

        public async Task UpdateAsync(IEnumerable<CircleCardEntity> entities)
        {
            Cards.UpdateRange(entities);
            await SaveChangesAsync();
        }
    }
}