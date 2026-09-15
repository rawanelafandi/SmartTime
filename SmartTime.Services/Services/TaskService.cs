using SmartTime.Repository.Entities;
using SmartTime.Repository.Interfaces;
using SmartTime.Services.Interfaces;

namespace SmartTime.Services.Services;

public class TaskService : ITaskService
{
    private readonly IUnitOfWork _unitOfWork;

    public TaskService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<WorkTask>> GetAllAsync() =>
        await _unitOfWork.Repository<WorkTask>().GetAllAsync(t => t.Project, t => t.AssignedUser);

    public async Task<WorkTask?> GetByIdAsync(int id) =>
        await _unitOfWork.Repository<WorkTask>().GetByIdAsync(id);

    public async Task<WorkTask> CreateAsync(string name, decimal estimateHours, int projectId, int assignedUserId)
    {
        var task = new WorkTask
        {
            Name = name,
            EstimateHours = estimateHours,
            ProjectId = projectId,
            AssignedUserId = assignedUserId
        };
        await _unitOfWork.Repository<WorkTask>().AddAsync(task);
        await _unitOfWork.SaveChangesAsync();
        return task;
    }

    public async Task<WorkTask?> UpdateAsync(int id, string name, decimal estimateHours, int projectId, int assignedUserId)
    {
        var task = await _unitOfWork.Repository<WorkTask>().GetByIdAsync(id);
        if (task is null) return null;

        task.Name = name;
        task.EstimateHours = estimateHours;
        task.ProjectId = projectId;
        task.AssignedUserId = assignedUserId;
        _unitOfWork.Repository<WorkTask>().Update(task);
        await _unitOfWork.SaveChangesAsync();
        return task;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _unitOfWork.Repository<WorkTask>().GetByIdAsync(id);
        if (task is null) return false;

        _unitOfWork.Repository<WorkTask>().Remove(task);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}