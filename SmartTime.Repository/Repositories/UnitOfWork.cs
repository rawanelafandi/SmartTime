using SmartTime.Repository.Data;
using SmartTime.Repository.Entities;
using SmartTime.Repository.Interfaces;

namespace SmartTime.Repository.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly SmartTimeDbContext _context;

    public UnitOfWork(SmartTimeDbContext context)
    {
        _context = context;
        Users = new Repository<AppUser>(_context);
        Projects = new Repository<Project>(_context);
        Tasks = new Repository<WorkTask>(_context);
        TimeEntries = new Repository<TimeEntry>(_context);
    }

    public IRepository<AppUser> Users { get; }
    public IRepository<Project> Projects { get; }
    public IRepository<WorkTask> Tasks { get; }
    public IRepository<TimeEntry> TimeEntries { get; }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}