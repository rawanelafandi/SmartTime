using SmartTime.Repository.Entities;
using SmartTime.Repository.Interfaces;
using SmartTime.Services.Interfaces;

namespace SmartTime.Services.Services;

public class TimeTrackingService : ITimeTrackingService
{
    private readonly IClockifyService _clockify;
    private readonly IUnitOfWork _unitOfWork;

    public TimeTrackingService(IClockifyService clockify, IUnitOfWork unitOfWork)
    {
        _clockify = clockify;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> SyncSampleDataAsync()
    {
        var apiOwnerClockifyUserId = await _clockify.GetCurrentUserIdAsync();

        // 1. Users
        var unsyncedUsers = await _unitOfWork.Repository<AppUser>().FindAsync(u => u.ClockifyUserId == null);
        foreach (var user in unsyncedUsers)
        {
            var clockifyUserId = await _clockify.FindUserIdByEmailAsync(user.Email);
            if (clockifyUserId is not null)
            {
                user.ClockifyUserId = clockifyUserId;
                _unitOfWork.Repository<AppUser>().Update(user);
            }
        }
        await _unitOfWork.SaveChangesAsync();

        // 2. Projects
        var unsyncedProjects = await _unitOfWork.Repository<Project>().FindAsync(p => p.ClockifyProjectId == null);
        foreach (var project in unsyncedProjects)
        {
            project.ClockifyProjectId = await _clockify.EnsureProjectAsync(project.Name);
            _unitOfWork.Repository<Project>().Update(project);
        }
        await _unitOfWork.SaveChangesAsync();

        // 3. Tasks
        var unsyncedTasks = await _unitOfWork.Repository<WorkTask>().FindAsync(t => t.ClockifyTaskId == null);
        foreach (var task in unsyncedTasks)
        {
            var project = await _unitOfWork.Repository<Project>().GetByIdAsync(task.ProjectId);
            var assignee = await _unitOfWork.Repository<AppUser>().GetByIdAsync(task.AssignedUserId);

            if (project?.ClockifyProjectId is null)
            {
                continue;
            }

            task.ClockifyTaskId = await _clockify.CreateTaskAsync(
                project.ClockifyProjectId,
                task.Name,
                task.EstimateHours,
                assignee?.ClockifyUserId);

            _unitOfWork.Repository<WorkTask>().Update(task);
        }
        await _unitOfWork.SaveChangesAsync();

        // 4. Time entries
        var unsyncedEntries = await _unitOfWork.Repository<TimeEntry>().FindAsync(e => e.ClockifyTimeEntryId == null);
        var entriesCreated = 0;

        foreach (var entry in unsyncedEntries)
        {
            var task = await _unitOfWork.Repository<WorkTask>().GetByIdAsync(entry.TaskId);
            if (task?.ClockifyTaskId is null)
            {
                continue;
            }

            var project = await _unitOfWork.Repository<Project>().GetByIdAsync(task.ProjectId);
            if (project?.ClockifyProjectId is null)
            {
                continue;
            }

            entry.ClockifyTimeEntryId = await _clockify.CreateTimeEntryAsync(
                apiOwnerClockifyUserId,
                project.ClockifyProjectId,
                task.ClockifyTaskId,
                entry.Start,
                entry.End,
                $"{task.Name} ({project.Name})");

            _unitOfWork.Repository<TimeEntry>().Update(entry);
            entriesCreated++;
        }
        await _unitOfWork.SaveChangesAsync();

        return entriesCreated;
    }
}