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
        public DbSet<Article> Articles { get; set; }
        public DbSet<CampaignMessage> CampaignMessages { get; set; }
        public DbSet<ComplianceReminder> ComplianceReminders { get; set; }
        public DbSet<LeaderboardScore> LeaderboardScores { get; set; }
        public DbSet<OtpVerificationCode> OtpVerificationCodes { get; set; }
        public DbSet<ElectionResult> ElectionResults { get; set; }
        public DbSet<CampaignGeoPing> CampaignGeoPings { get; set; }
        public DbSet<AuditLogEvent> AuditLogEvents { get; set; }

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
            modelBuilder.Entity<Article>().ToTable("Articles");
            modelBuilder.Entity<CampaignMessage>().ToTable("CampaignMessages");
            modelBuilder.Entity<ComplianceReminder>().ToTable("ComplianceReminders");
            modelBuilder.Entity<LeaderboardScore>().ToTable("LeaderboardScores");
            modelBuilder.Entity<OtpVerificationCode>().ToTable("OtpVerificationCodes");
            modelBuilder.Entity<ElectionResult>().ToTable("ElectionResults");
            modelBuilder.Entity<CampaignGeoPing>().ToTable("CampaignGeoPings");
            modelBuilder.Entity<AuditLogEvent>().ToTable("AuditLogEvents");

            // Configure indexes for better query performance
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique()
                .HasFilter("\"PhoneNumber\" IS NOT NULL AND \"PhoneNumber\" <> ''");

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => new { u.CampaignRole, u.Region, u.County, u.VerificationStatus, u.VoterCardStatus });

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.ParentUserId);

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

            modelBuilder.Entity<Models.Task>()
                .HasIndex(t => t.AssignedToUserId);
            
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
            
            // Configure indexes for Articles
            modelBuilder.Entity<Article>()
                .HasIndex(a => a.Slug)
                .IsUnique();
            
            modelBuilder.Entity<Article>()
                .HasIndex(a => a.Status);
            
            modelBuilder.Entity<Article>()
                .HasIndex(a => a.Category);
            
            modelBuilder.Entity<Article>()
                .HasIndex(a => a.PublishedDate);
            
            modelBuilder.Entity<Article>()
                .HasIndex(a => a.IsFeatured);

            modelBuilder.Entity<CampaignMessage>()
                .HasIndex(m => new { m.Channel, m.Status, m.CreatedAt });

            modelBuilder.Entity<CampaignMessage>()
                .HasIndex(m => new { m.Status, m.NextAttemptAt, m.RetryCount });

            modelBuilder.Entity<ComplianceReminder>()
                .HasIndex(r => new { r.UserId, r.CreatedAt });

            modelBuilder.Entity<LeaderboardScore>()
                .HasIndex(l => new { l.Scope, l.Region, l.County, l.TotalPoints });

            modelBuilder.Entity<OtpVerificationCode>()
                .HasIndex(o => new { o.UserId, o.Purpose, o.ExpiresAt, o.IsUsed });

            modelBuilder.Entity<ElectionResult>()
                .HasIndex(r => new { r.ReportingWindow, r.PollingStationCode });

            modelBuilder.Entity<ElectionResult>()
                .HasIndex(r => new { r.ReportingWindow, r.County, r.Constituency });

            modelBuilder.Entity<ElectionResult>()
                .HasIndex(r => new { r.SubmittedByUserId, r.ReportingWindow, r.PollingStationCode })
                .IsUnique();

            modelBuilder.Entity<CampaignGeoPing>()
                .HasIndex(p => new { p.CapturedAt, p.Region, p.County });

            modelBuilder.Entity<CampaignGeoPing>()
                .HasIndex(p => new { p.UserId, p.CapturedAt });

            modelBuilder.Entity<AuditLogEvent>()
                .HasIndex(a => new { a.EventType, a.CreatedAt });

            modelBuilder.Entity<AuditLogEvent>()
                .HasIndex(a => new { a.ActorUserId, a.CreatedAt });

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.ParentUser)
                .WithMany(u => u.DirectDownlines)
                .HasForeignKey(u => u.ParentUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.Task>()
                .HasOne(t => t.AssignedToUser)
                .WithMany()
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.Task>()
                .HasOne(t => t.CreatedByUser)
                .WithMany()
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.Task>()
                .HasOne(t => t.AssignedByUser)
                .WithMany()
                .HasForeignKey(t => t.AssignedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CampaignMessage>()
                .HasOne(m => m.SenderUser)
                .WithMany()
                .HasForeignKey(m => m.SenderUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CampaignMessage>()
                .HasOne(m => m.ReceiverUser)
                .WithMany()
                .HasForeignKey(m => m.ReceiverUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ComplianceReminder>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LeaderboardScore>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OtpVerificationCode>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ElectionResult>()
                .HasOne(r => r.SubmittedByUser)
                .WithMany()
                .HasForeignKey(r => r.SubmittedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CampaignGeoPing>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
