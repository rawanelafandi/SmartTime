using SmartTime.Repository.Entities;
using SmartTime.Repository.Interfaces;
using SmartTime.Services.Interfaces;

namespace SmartTime.Services.Services;

public class TimeEntryService : ITimeEntryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClockifyService _clockify;

    public TimeEntryService(IUnitOfWork unitOfWork, IClockifyService clockify)
    {
        _unitOfWork = unitOfWork;
        _clockify = clockify;
    }

    public async Task<IReadOnlyList<TimeEntry>> GetAllAsync() =>
        await _unitOfWork.Repository<TimeEntry>().GetAllAsync(e => e.Task, e => e.Task.Project, e => e.Task.AssignedUser);

    public async Task<TimeEntry?> GetByIdAsync(int id) =>
        await _unitOfWork.Repository<TimeEntry>().GetByIdAsync(id);

    public async Task<TimeEntry> CreateAsync(int taskId, DateTime start, DateTime end)
    {
        var task = await _unitOfWork.Repository<WorkTask>().GetByIdAsync(taskId)
            ?? throw new InvalidOperationException($"Task {taskId} not found.");
        var project = await _unitOfWork.Repository<Project>().GetByIdAsync(task.ProjectId)
            ?? throw new InvalidOperationException($"Project for task {taskId} not found.");

        if (task.ClockifyTaskId is null || project.ClockifyProjectId is null)
        {
            throw new InvalidOperationException($"Task '{task.Name}' or its project has not been synced to Clockify yet.");
        }

        var apiOwnerClockifyUserId = await _clockify.GetCurrentUserIdAsync();

        var clockifyTimeEntryId = await _clockify.CreateTimeEntryAsync(
            apiOwnerClockifyUserId, project.ClockifyProjectId, task.ClockifyTaskId, start, end,
            $"{task.Name} ({project.Name})");

        var entry = new TimeEntry { TaskId = taskId, Start = start, End = end, ClockifyTimeEntryId = clockifyTimeEntryId };
        await _unitOfWork.Repository<TimeEntry>().AddAsync(entry);
        await _unitOfWork.SaveChangesAsync();
        return entry;
    }

    public async Task<TimeEntry?> UpdateAsync(int id, int taskId, DateTime start, DateTime end)
    {
        var entry = await _unitOfWork.Repository<TimeEntry>().GetByIdAsync(id);
        if (entry is null) return null;

        if (entry.ClockifyTimeEntryId is not null)
        {
            var apiOwnerClockifyUserId = await _clockify.GetCurrentUserIdAsync();
            await _clockify.UpdateTimeEntryAsync(apiOwnerClockifyUserId, entry.ClockifyTimeEntryId, start, end);
        }

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