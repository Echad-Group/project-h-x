using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Models;

namespace NewKenyaAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<EventRSVP> EventRSVPs { get; set; }
        public DbSet<PushSubscription> PushSubscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure table names
            modelBuilder.Entity<Volunteer>().ToTable("Volunteers");
            modelBuilder.Entity<Contact>().ToTable("Contacts");
            modelBuilder.Entity<Donation>().ToTable("Donations");
            modelBuilder.Entity<EventRSVP>().ToTable("EventRSVPs");
            modelBuilder.Entity<PushSubscription>().ToTable("PushSubscriptions");

            // Configure indexes for better query performance
            modelBuilder.Entity<Volunteer>()
                .HasIndex(v => v.Email)
                .IsUnique();

            modelBuilder.Entity<EventRSVP>()
                .HasIndex(e => e.EventId);

            modelBuilder.Entity<Donation>()
                .HasIndex(d => d.TransactionId);
            
            modelBuilder.Entity<PushSubscription>()
                .HasIndex(p => p.Endpoint)
                .IsUnique();
        }
    }
}
