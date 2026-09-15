using SmartTime.Repository.Entities;
using SmartTime.Repository.Interfaces;
using SmartTime.Services.Interfaces;

namespace SmartTime.Services.Services;

// Scans the local database for records that haven't been pushed to Clockify yet
// (identified by a null ClockifyXId) and syncs just those - so you can add
// users/projects/tasks/time-entries via normal CRUD endpoints, then call this
// to push whatever's new since the last sync.
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
        // Clockify's free/trial plan only allows creating time entries for the
        // account that owns the API key - every entry is pushed under this
        // user on Clockify, while the correct assignee stays on Task.AssignedUser locally.
        var apiOwnerClockifyUserId = await _clockify.GetCurrentUserIdAsync();

        // 1. Users: try to resolve a ClockifyUserId for anyone missing one.
        var unsyncedUsers = await _unitOfWork.Users.FindAsync(u => u.ClockifyUserId == null);
        foreach (var user in unsyncedUsers)
        {
            var clockifyUserId = await _clockify.FindUserIdByEmailAsync(user.Email);
            if (clockifyUserId is not null)
            {
                user.ClockifyUserId = clockifyUserId;
                _unitOfWork.Users.Update(user);
            }
        }
        await _unitOfWork.SaveChangesAsync();

        // 2. Projects: create/find on Clockify for anyone missing a ClockifyProjectId.
        var unsyncedProjects = await _unitOfWork.Projects.FindAsync(p => p.ClockifyProjectId == null);
        foreach (var project in unsyncedProjects)
        {
            project.ClockifyProjectId = await _clockify.EnsureProjectAsync(project.Name);
            _unitOfWork.Projects.Update(project);
        }
        await _unitOfWork.SaveChangesAsync();

        // 3. Tasks: needs its project already synced.
        var unsyncedTasks = await _unitOfWork.Tasks.FindAsync(t => t.ClockifyTaskId == null);
        foreach (var task in unsyncedTasks)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId);
            var assignee = await _unitOfWork.Users.GetByIdAsync(task.AssignedUserId);

            if (project?.ClockifyProjectId is null)
            {
                continue; // project isn't synced yet - skip until it is
            }

            task.ClockifyTaskId = await _clockify.CreateTaskAsync(
                project.ClockifyProjectId,
                task.Name,
                task.EstimateHours,
                assignee?.ClockifyUserId);

            _unitOfWork.Tasks.Update(task);
        }
        await _unitOfWork.SaveChangesAsync();

        // 4. Time entries: needs its task already synced.
        var unsyncedEntries = await _unitOfWork.TimeEntries.FindAsync(e => e.ClockifyTimeEntryId == null);
        var entriesCreated = 0;

        foreach (var entry in unsyncedEntries)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(entry.TaskId);
            if (task?.ClockifyTaskId is null)
            {
                continue; // task isn't synced yet - skip until it is
            }

            var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId);
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

            _unitOfWork.TimeEntries.Update(entry);
            entriesCreated++;
        }
        await _unitOfWork.SaveChangesAsync();

        return entriesCreated;
    }
}