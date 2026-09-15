namespace SmartTime.Repository.Entities;

public class AppUser
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Id of the corresponding user on Clockify, once synced.
    public string? ClockifyUserId { get; set; }

    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}