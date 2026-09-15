using SmartTime.Repository.Entities;

namespace SmartTime.Services.Interfaces;

public interface ITaskService
{
    Task<IReadOnlyList<WorkTask>> GetAllAsync();
    Task<WorkTask?> GetByIdAsync(int id);
    Task<WorkTask> CreateAsync(string name, decimal estimateHours, int projectId, int assignedUserId);
    Task<WorkTask?> UpdateAsync(int id, string name, decimal estimateHours, int projectId, int assignedUserId);
    Task<bool> DeleteAsync(int id);
}