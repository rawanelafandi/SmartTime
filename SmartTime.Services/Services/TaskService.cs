using SmartTime.Repository.Entities;
using SmartTime.Repository.Interfaces;
using SmartTime.Services.Interfaces;

namespace SmartTime.Services.Services;

public class TaskService : ITaskService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClockifyService _clockify;

    public TaskService(IUnitOfWork unitOfWork, IClockifyService clockify)
    {
        _unitOfWork = unitOfWork;
        _clockify = clockify;
    }

    public async Task<IReadOnlyList<WorkTask>> GetAllAsync() =>
        await _unitOfWork.Repository<WorkTask>().GetAllAsync(t => t.Project, t => t.AssignedUser);

    public async Task<WorkTask?> GetByIdAsync(int id) =>
        await _unitOfWork.Repository<WorkTask>().GetByIdAsync(id);

    public async Task<WorkTask> CreateAsync(string name, decimal estimateHours, int projectId, int assignedUserId)
    {
        var project = await _unitOfWork.Repository<Project>().GetByIdAsync(projectId)
            ?? throw new InvalidOperationException($"Project {projectId} not found.");
        var user = await _unitOfWork.Repository<AppUser>().GetByIdAsync(assignedUserId)
            ?? throw new InvalidOperationException($"User {assignedUserId} not found.");

        if (project.ClockifyProjectId is null)
        {
            throw new InvalidOperationException($"Project '{project.Name}' has not been synced to Clockify yet.");
        }

        var clockifyTaskId = await _clockify.CreateTaskAsync(
            project.ClockifyProjectId, name, estimateHours, user.ClockifyUserId);

        var task = new WorkTask
        {
            Name = name,
            EstimateHours = estimateHours,
            ProjectId = projectId,
            AssignedUserId = assignedUserId,
            ClockifyTaskId = clockifyTaskId
        };
        await _unitOfWork.Repository<WorkTask>().AddAsync(task);
        await _unitOfWork.SaveChangesAsync();
        return task;
    }

    public async Task<WorkTask?> UpdateAsync(int id, string name, decimal estimateHours, int projectId, int assignedUserId)
    {
        var task = await _unitOfWork.Repository<WorkTask>().GetByIdAsync(id);
        if (task is null) return null;

        var project = await _unitOfWork.Repository<Project>().GetByIdAsync(projectId)
            ?? throw new InvalidOperationException($"Project {projectId} not found.");

        if (task.ClockifyTaskId is not null && project.ClockifyProjectId is not null)
        {
            await _clockify.UpdateTaskAsync(project.ClockifyProjectId, task.ClockifyTaskId, name, estimateHours);
        }

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