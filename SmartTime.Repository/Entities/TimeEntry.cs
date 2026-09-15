namespace SmartTime.Repository.Entities;

public class TimeEntry
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public AppUser User { get; set; } = null!;

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public int TaskId { get; set; }
    public WorkTask Task { get; set; } = null!;

    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public string? ClockifyTimeEntryId { get; set; }

    public double DurationHours => (End - Start).TotalHours;
}