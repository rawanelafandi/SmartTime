using SmartTime.Repository.Data;
using SmartTime.Repository.Interfaces;

namespace SmartTime.Repository.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly SmartTimeDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(SmartTimeDbContext context)
    {
        _context = context;
    }

    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new Repository<T>(_context);
        }
        return (IRepository<T>)_repositories[type];
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}