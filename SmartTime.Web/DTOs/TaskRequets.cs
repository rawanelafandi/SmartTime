namespace SmartTime.Web.DTOs;

public record CreateTaskRequest(string Name, decimal EstimateHours, int ProjectId, int AssignedUserId);
public record UpdateTaskRequest(string Name, decimal EstimateHours, int ProjectId, int AssignedUserId);