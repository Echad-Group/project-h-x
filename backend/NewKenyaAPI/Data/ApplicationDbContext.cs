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
        public DbSet<Unit> Units { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<VolunteerAssignment> VolunteerAssignments { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure table names
            modelBuilder.Entity<Volunteer>().ToTable("Volunteers");
            modelBuilder.Entity<Contact>().ToTable("Contacts");
            modelBuilder.Entity<Donation>().ToTable("Donations");
            modelBuilder.Entity<EventRSVP>().ToTable("EventRSVPs");
            modelBuilder.Entity<PushSubscription>().ToTable("PushSubscriptions");
            modelBuilder.Entity<Unit>().ToTable("Units");
            modelBuilder.Entity<Team>().ToTable("Teams");
            modelBuilder.Entity<VolunteerAssignment>().ToTable("VolunteerAssignments");
            modelBuilder.Entity<Models.Task>().ToTable("Tasks");

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
            
            // Configure indexes for Units & Teams
            modelBuilder.Entity<Unit>()
                .HasIndex(u => u.DisplayOrder);
            
            modelBuilder.Entity<Team>()
                .HasIndex(t => new { t.UnitId, t.DisplayOrder });
            
            modelBuilder.Entity<VolunteerAssignment>()
                .HasIndex(va => new { va.VolunteerId, va.UnitId, va.TeamId });
            
            modelBuilder.Entity<Models.Task>()
                .HasIndex(t => new { t.Status, t.DueDate });
            
            modelBuilder.Entity<Models.Task>()
                .HasIndex(t => t.Region);
            
            // Configure relationships
            modelBuilder.Entity<Team>()
                .HasOne(t => t.Unit)
                .WithMany(u => u.Teams)
                .HasForeignKey(t => t.UnitId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<VolunteerAssignment>()
                .HasOne(va => va.Volunteer)
                .WithMany()
                .HasForeignKey(va => va.VolunteerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<VolunteerAssignment>()
                .HasOne(va => va.Unit)
                .WithMany(u => u.VolunteerAssignments)
                .HasForeignKey(va => va.UnitId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<VolunteerAssignment>()
                .HasOne(va => va.Team)
                .WithMany(t => t.VolunteerAssignments)
                .HasForeignKey(va => va.TeamId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
