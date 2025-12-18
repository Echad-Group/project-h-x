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
        public DbSet<Event> Events { get; set; }
        public DbSet<EventRSVP> EventRSVPs { get; set; }
        public DbSet<PushSubscription> PushSubscriptions { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<VolunteerAssignment> VolunteerAssignments { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<IssueInitiative> IssueInitiatives { get; set; }
        public DbSet<IssueQuestion> IssueQuestions { get; set; }
        public DbSet<CampaignTeamMember> CampaignTeamMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure table names
            modelBuilder.Entity<Volunteer>().ToTable("Volunteers");
            modelBuilder.Entity<Contact>().ToTable("Contacts");
            modelBuilder.Entity<Donation>().ToTable("Donations");
            modelBuilder.Entity<Event>().ToTable("Events");
            modelBuilder.Entity<EventRSVP>().ToTable("EventRSVPs");
            modelBuilder.Entity<PushSubscription>().ToTable("PushSubscriptions");
            modelBuilder.Entity<Unit>().ToTable("Units");
            modelBuilder.Entity<Team>().ToTable("Teams");
            modelBuilder.Entity<VolunteerAssignment>().ToTable("VolunteerAssignments");
            modelBuilder.Entity<Models.Task>().ToTable("Tasks");
            modelBuilder.Entity<Issue>().ToTable("Issues");
            modelBuilder.Entity<IssueInitiative>().ToTable("IssueInitiatives");
            modelBuilder.Entity<IssueQuestion>().ToTable("IssueQuestions");
            modelBuilder.Entity<CampaignTeamMember>().ToTable("CampaignTeamMembers");

            // Configure indexes for better query performance
            modelBuilder.Entity<Volunteer>()
                .HasIndex(v => v.Email)
                .IsUnique();

            modelBuilder.Entity<EventRSVP>()
                .HasIndex(e => e.EventId);
            
            // Configure indexes for Events
            modelBuilder.Entity<Event>()
                .HasIndex(e => e.Slug)
                .IsUnique();

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
            
            // Configure indexes for Issues
            modelBuilder.Entity<Issue>()
                .HasIndex(i => i.Slug)
                .IsUnique();
            
            modelBuilder.Entity<Issue>()
                .HasIndex(i => i.DisplayOrder);
            
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
            
            // Configure relationships for Issues
            modelBuilder.Entity<IssueInitiative>()
                .HasOne(ii => ii.Issue)
                .WithMany(i => i.Initiatives)
                .HasForeignKey(ii => ii.IssueId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<IssueQuestion>()
                .HasOne(iq => iq.Issue)
                .WithMany(i => i.Questions)
                .HasForeignKey(iq => iq.IssueId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Configure relationships for Events
            modelBuilder.Entity<EventRSVP>()
                .HasOne(r => r.Event)
                .WithMany(e => e.RSVPs)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
