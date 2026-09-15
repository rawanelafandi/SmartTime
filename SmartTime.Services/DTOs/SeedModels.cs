namespace SmartTime.Services.DTOs;

// Plain in-memory shape of the sample dataset from the task sheet -
// not EF entities, so the seeder stays decoupled from persistence.
public record SeedTask(
    string Project,
    string TaskName,
    string AssignedUserName,
    decimal EstimateHours);

public record SeedTimeEntry(
    string UserName,
    string Project,
    string TaskName,
    DateTime Start,
    DateTime End);