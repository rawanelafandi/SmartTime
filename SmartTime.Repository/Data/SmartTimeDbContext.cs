using Microsoft.EntityFrameworkCore;
using SmartTime.Repository.Entities;

namespace SmartTime.Repository.Data;

public class SmartTimeDbContext : DbContext
{
    public SmartTimeDbContext(DbContextOptions<SmartTimeDbContext> options) : base(options) { }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<WorkTask> Tasks => Set<WorkTask>();
    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<WorkTask>(e =>
        {
            e.HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(t => t.AssignedUser)
                .WithMany()
                .HasForeignKey(t => t.AssignedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Property(t => t.EstimateHours).HasColumnType("decimal(6,2)");
        });

        modelBuilder.Entity<TimeEntry>(e =>
        {
            e.HasOne(te => te.User)
                .WithMany(u => u.TimeEntries)
                .HasForeignKey(te => te.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(te => te.Project)
                .WithMany()
                .HasForeignKey(te => te.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(te => te.Task)
                .WithMany(t => t.TimeEntries)
                .HasForeignKey(te => te.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}