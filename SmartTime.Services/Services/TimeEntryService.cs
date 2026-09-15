using SmartTime.Repository.Entities;
using SmartTime.Repository.Interfaces;
using SmartTime.Services.Interfaces;

namespace SmartTime.Services.Services;

public class TimeEntryService : ITimeEntryService
{
    private readonly IUnitOfWork _unitOfWork;

    public TimeEntryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<TimeEntry>> GetAllAsync() =>
        await _unitOfWork.Repository<TimeEntry>().GetAllAsync(e => e.Task, e => e.Task.Project, e => e.Task.AssignedUser);

    public async Task<TimeEntry?> GetByIdAsync(int id) =>
        await _unitOfWork.Repository<TimeEntry>().GetByIdAsync(id);

    public async Task<TimeEntry> CreateAsync(int taskId, DateTime start, DateTime end)
    {
        var entry = new TimeEntry { TaskId = taskId, Start = start, End = end };
        await _unitOfWork.Repository<TimeEntry>().AddAsync(entry);
        await _unitOfWork.SaveChangesAsync();
        return entry;
    }

    public async Task<TimeEntry?> UpdateAsync(int id, int taskId, DateTime start, DateTime end)
    {
        var entry = await _unitOfWork.Repository<TimeEntry>().GetByIdAsync(id);
        if (entry is null) return null;

        entry.TaskId = taskId;
        entry.Start = start;
        entry.End = end;
        _unitOfWork.Repository<TimeEntry>().Update(entry);
        await _unitOfWork.SaveChangesAsync();
        return entry;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entry = await _unitOfWork.Repository<TimeEntry>().GetByIdAsync(id);
        if (entry is null) return false;

        _unitOfWork.Repository<TimeEntry>().Remove(entry);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}