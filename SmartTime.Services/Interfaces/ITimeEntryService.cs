using SmartTime.Repository.Entities;

namespace SmartTime.Services.Interfaces;

public interface ITimeEntryService
{
    Task<IReadOnlyList<TimeEntry>> GetAllAsync();
    Task<TimeEntry?> GetByIdAsync(int id);
    Task<TimeEntry> CreateAsync(int taskId, DateTime start, DateTime end);
    Task<TimeEntry?> UpdateAsync(int id, int taskId, DateTime start, DateTime end);
    Task<bool> DeleteAsync(int id);
}