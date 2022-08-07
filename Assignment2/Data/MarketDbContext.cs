using Assignment2.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment2.Data
{
    public class MarketDbContext : DbContext
    {

        
            public MarketDbContext(DbContextOptions<MarketDbContext> options) : base(options)
            {
            }

            public DbSet<Client> Clients { get; set; }
            public DbSet<Brokerage> Brokerages { get; set; }
            public DbSet<Subscription> Subscriptions { get; set; }
            public DbSet<Advertisement> Advertisements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
            modelBuilder.Entity<Client>().ToTable("clients");
            modelBuilder.Entity<Client>().HasMany(b => b.Subscriptions).WithOne(b => b.Client);

            modelBuilder.Entity<Brokerage>().ToTable("brokerages");
            modelBuilder.Entity<Brokerage>().HasMany(b => b.Subscriptions).WithOne(b => b.Brokerage);

            modelBuilder.Entity<Subscription>().ToTable("subscriptions");
            modelBuilder.Entity<Advertisement>().ToTable("advertisements");
            
            modelBuilder.Entity<Subscription>().HasOne(b => b.Brokerage).WithMany(b => b.Subscriptions).HasForeignKey(b => b.BrokerageId);
            modelBuilder.Entity<Subscription>().HasOne(b => b.Client).WithMany(b => b.Subscriptions).HasForeignKey(b => b.ClientId);

            /* composite key decleration */
            modelBuilder.Entity<Subscription>().HasKey(c => new { c.ClientId, c.BrokerageId });
            modelBuilder.Entity<Brokerage>().HasMany(c => c.Advertisements);




        }
    }
    
}
