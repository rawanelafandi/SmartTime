namespace SmartTime.Repository.Entities;

public class TimeEntry
{
    public int Id { get; set; }

    public int TaskId { get; set; }
    public WorkTask Task { get; set; } = null!;

    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public string? ClockifyTimeEntryId { get; set; }

    public double DurationHours => (End - Start).TotalHours;
}