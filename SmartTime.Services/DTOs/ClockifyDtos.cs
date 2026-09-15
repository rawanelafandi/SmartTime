using System.Text.Json.Serialization;

namespace SmartTime.Services.DTOs;

public record ClockifyUserDto
{
    [JsonPropertyName("id")] public string Id { get; init; } = string.Empty;
    [JsonPropertyName("email")] public string Email { get; init; } = string.Empty;
    [JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
}

public record ClockifyProjectDto
{
    [JsonPropertyName("id")] public string Id { get; init; } = string.Empty;
    [JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
}

public record ClockifyTaskDto
{
    [JsonPropertyName("id")] public string Id { get; init; } = string.Empty;
    [JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
}

public record ClockifyTimeEntryDto
{
    [JsonPropertyName("id")] public string Id { get; init; } = string.Empty;
}

// Clockify's list endpoints wrap results as { "value": [...], "Count": N }
// instead of returning a bare JSON array.
public record ClockifyPagedResult<T>
{
    [JsonPropertyName("list")] public List<T> Value { get; init; } = new();
}

public record CreateProjectRequest([property: JsonPropertyName("name")] string Name);

public record CreateTaskRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("estimate")] string Estimate,
    [property: JsonPropertyName("assigneeIds")] List<string> AssigneeIds);

public record CreateTimeEntryRequest(
    [property: JsonPropertyName("start")] string Start,
    [property: JsonPropertyName("end")] string End,
    [property: JsonPropertyName("projectId")] string ProjectId,
    [property: JsonPropertyName("taskId")] string TaskId,
    [property: JsonPropertyName("description")] string Description);