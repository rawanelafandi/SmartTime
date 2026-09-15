using SmartTime.Repository.Entities;

namespace SmartTime.Repository.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<AppUser> Users { get; }
    IRepository<Project> Projects { get; }
    IRepository<WorkTask> Tasks { get; }
    IRepository<TimeEntry> TimeEntries { get; }

    Task<int> SaveChangesAsync();
}