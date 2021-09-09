using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamuraiApp.Domain;

namespace SamuraiApp.Data
{
    public class SamuraiContext : DbContext
    {
        public DbSet<Samurai> Samurais { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Battle> Battles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                    "Server=tcp:nsdfazaltest.database.windows.net,1433;Initial Catalog=SamuraiAppData;Persist Security Info=False;" +
                    "User ID=ffarruq;Password=Y@hoo89a!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")
                .LogTo(Console.WriteLine, new[]
                    {
                        DbLoggerCategory.Database.Command.Name , DbLoggerCategory.Database.Transaction.Name // logs the query send to the database
                    },
                    LogLevel.Information // logs just the query information .. this is one more filter you can pass to make it the log less verbose
                ).EnableSensitiveDataLogging(); // this will output the input parameters for inserts and updates;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<Samurai>()
            //     .HasMany(s => s.Battles)
            //     .WithMany(b => b.Samurais)
            //     .UsingEntity<BattleSamurai>(
            //         bs => bs.HasOne<Battle>().WithMany(),
            //         bs => bs.HasOne<Samurai>().WithMany())
            //     .Property(bs => bs.DateJoined)
            //     .HasDefaultValueSql("getdate()");
        }
    }
}