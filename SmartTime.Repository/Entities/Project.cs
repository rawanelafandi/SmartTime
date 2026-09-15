namespace SmartTime.Repository.Entities;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string? ClockifyProjectId { get; set; }

    public ICollection<WorkTask> Tasks { get; set; } = new List<WorkTask>();
}