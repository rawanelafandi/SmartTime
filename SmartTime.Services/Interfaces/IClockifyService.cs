namespace SmartTime.Services.Interfaces;

public interface IClockifyService
{
    Task<string?> FindUserIdByEmailAsync(string email);
    Task<string> EnsureProjectAsync(string projectName);
    Task<string> CreateTaskAsync(string clockifyProjectId, string taskName, decimal estimateHours, string? assigneeClockifyUserId);
    Task<string> CreateTimeEntryAsync(string clockifyUserId, string clockifyProjectId, string clockifyTaskId, DateTime start, DateTime end, string description);
}