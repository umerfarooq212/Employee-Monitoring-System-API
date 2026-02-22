using Employee_Monitoring_System_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Employee_Monitoring_System_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Branch → HeadUser (one-to-one/optional)
            modelBuilder.Entity<Branch>()
                .HasOne(b => b.HeadUser)
                .WithMany()
                .HasForeignKey(b => b.HeadUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Task → Project (optional)
            modelBuilder.Entity<_Task>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            // Task → AssignedUser (optional)
            modelBuilder.Entity<_Task>()
                .HasOne(t => t.AssignedUser)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.AssignedTo)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            // Screenshot → User (optional)
            modelBuilder.Entity<Screenshot>()
                .HasOne(s => s.User)
                .WithMany(u => u.Screenshots)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            // ActivityLog → User (optional)
            modelBuilder.Entity<ActivityLog>()
                .HasOne(a => a.User)
                .WithMany(u => u.ActivityLogs)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            // LeaveRequest → User (optional)
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(l => l.User)
                .WithMany(u => u.LeaveRequests)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            // Notification → User (optional)
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            // UserTask many-to-many
            modelBuilder.Entity<UserTask>()
                .HasKey(ut => new { ut.UserId, ut.TaskId });

            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTasks)
                .HasForeignKey(ut => ut.UserId);

            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.Task)
                .WithMany(t => t.UserTasks)
                .HasForeignKey(ut => ut.TaskId);

            // ProjectMember many-to-many
            modelBuilder.Entity<ProjectMember>()
                .HasKey(pm => new { pm.UserId, pm.ProjectId });

            // TechnicalSkills → store as JSON string
            modelBuilder.Entity<User>()
                .Property(u => u.TechnicalSkills)
                .HasColumnType("nvarchar(max)");

            base.OnModelCreating(modelBuilder);

            // Convert Dictionary<string,string> to JSON string
            var converter = new ValueConverter<Dictionary<string, string>, string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null)
            );

            modelBuilder.Entity<User>()
                .Property(u => u.TechnicalSkills)
                .HasConversion(converter);

            modelBuilder.Entity<UserTask>()
        .HasKey(ut => new { ut.UserId, ut.TaskId });

            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTasks)
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Restrict); // <-- important, disables cascade

            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.Task)
                .WithMany(t => t.UserTasks)
                .HasForeignKey(ut => ut.TaskId)
                .OnDelete(DeleteBehavior.Cascade); // optional, safe
        }

        // ===== DbSets =====
        public DbSet<User> Users { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<_Task> Tasks { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Screenshot> Screenshots { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AppSettings> AppSettings { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<UserTask> UserTasks { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
    }
}