using Microsoft.AspNetCore.Mvc;
using SmartTime.Repository.Entities;
using SmartTime.Repository.Interfaces;
using SmartTime.Web.DTOs;

namespace SmartTime.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public TaskController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tasks = await _unitOfWork.Repository<WorkTask>().GetAllAsync(t => t.Project, t => t.AssignedUser);
        return Ok(tasks.Select(t => new
        {
            t.Id,
            t.Name,
            t.EstimateHours,
            Project = t.Project?.Name,
            AssignedUser = t.AssignedUser?.Name,
            t.ClockifyTaskId
        }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var task = await _unitOfWork.Repository<WorkTask>().GetByIdAsync(id);
        if (task is null) return NotFound();
        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
    {
        var task = new WorkTask
        {
            Name = request.Name,
            EstimateHours = request.EstimateHours,
            ProjectId = request.ProjectId,
            AssignedUserId = request.AssignedUserId
        };
        await _unitOfWork.Repository<WorkTask>().AddAsync(task);
        await _unitOfWork.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskRequest request)
    {
        var task = await _unitOfWork.Repository<WorkTask>().GetByIdAsync(id);
        if (task is null) return NotFound();

        task.Name = request.Name;
        task.EstimateHours = request.EstimateHours;
        task.ProjectId = request.ProjectId;
        task.AssignedUserId = request.AssignedUserId;
        _unitOfWork.Repository<WorkTask>().Update(task);
        await _unitOfWork.SaveChangesAsync();
        return Ok(task);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _unitOfWork.Repository<WorkTask>().GetByIdAsync(id);
        if (task is null) return NotFound();

        _unitOfWork.Repository<WorkTask>().Remove(task);
        await _unitOfWork.SaveChangesAsync();
        return NoContent();
    }
}