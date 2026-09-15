namespace SmartTime.Repository.Entities;

public class WorkTask
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal EstimateHours { get; set; }

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public int AssignedUserId { get; set; }
    public AppUser AssignedUser { get; set; } = null!;

    public string? ClockifyTaskId { get; set; }

    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}