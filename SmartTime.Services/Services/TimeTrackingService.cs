using SmartTime.Repository.Entities;
using SmartTime.Repository.Interfaces;
using SmartTime.Services.Interfaces;
using SmartTime.Services.SeedData;

namespace SmartTime.Services.Services;

// Orchestrates the whole "push sample data" workflow: for each user/project/
// task/time-entry in the sample set, push it to Clockify first, then persist
// the local record (with the returned Clockify id attached) via the Unit of Work.
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
        // account that owns the API key, so every entry is pushed under this
        // user on Clockify - the correct assignee is still stored locally via Task.AssignedUser.
        var apiOwnerClockifyUserId = await _clockify.GetCurrentUserIdAsync();

        var userCache = new Dictionary<string, AppUser>();
        var projectCache = new Dictionary<string, Project>();
        var taskCache = new Dictionary<(string Project, string Task), WorkTask>();

        foreach (var (userName, email) in SampleDataProvider.UserEmails)
        {
            var clockifyUserId = await _clockify.FindUserIdByEmailAsync(email);

            var user = new AppUser
            {
                Name = userName,
                Email = email,
                ClockifyUserId = clockifyUserId
            };

            await _unitOfWork.Users.AddAsync(user);
            userCache[userName] = user;
        }

        await _unitOfWork.SaveChangesAsync();

        foreach (var seedTask in SampleDataProvider.Tasks)
        {
            if (!projectCache.TryGetValue(seedTask.Project, out var project))
            {
                var clockifyProjectId = await _clockify.EnsureProjectAsync(seedTask.Project);
                project = new Project { Name = seedTask.Project, ClockifyProjectId = clockifyProjectId };
                await _unitOfWork.Projects.AddAsync(project);
                projectCache[seedTask.Project] = project;
            }

            var assignee = userCache[seedTask.AssignedUserName];

            var clockifyTaskId = await _clockify.CreateTaskAsync(
                project.ClockifyProjectId!,
                seedTask.TaskName,
                seedTask.EstimateHours,
                assignee.ClockifyUserId);

            var task = new WorkTask
            {
                Name = seedTask.TaskName,
                EstimateHours = seedTask.EstimateHours,
                Project = project,
                AssignedUser = assignee,
                ClockifyTaskId = clockifyTaskId
            };

            await _unitOfWork.Tasks.AddAsync(task);
            taskCache[(seedTask.Project, seedTask.TaskName)] = task;
        }

        await _unitOfWork.SaveChangesAsync();

        var entriesCreated = 0;
        foreach (var seedEntry in SampleDataProvider.TimeEntries)
        {
            var project = projectCache[seedEntry.Project];
            var task = taskCache[(seedEntry.Project, seedEntry.TaskName)];

            var clockifyTimeEntryId = await _clockify.CreateTimeEntryAsync(
                apiOwnerClockifyUserId,
                project.ClockifyProjectId!,
                task.ClockifyTaskId!,
                seedEntry.Start,
                seedEntry.End,
                $"{seedEntry.TaskName} ({seedEntry.Project})");

            var timeEntry = new TimeEntry
            {
                Task = task,
                Start = seedEntry.Start,
                End = seedEntry.End,
                ClockifyTimeEntryId = clockifyTimeEntryId
            };

            await _unitOfWork.TimeEntries.AddAsync(timeEntry);
            entriesCreated++;
        }

        await _unitOfWork.SaveChangesAsync();

        return entriesCreated;
    }
}