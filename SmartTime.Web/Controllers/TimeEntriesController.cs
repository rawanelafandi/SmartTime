using Microsoft.AspNetCore.Mvc;
using SmartTime.Repository.Entities;
using SmartTime.Repository.Interfaces;
using SmartTime.Web.DTOs;

namespace SmartTime.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeEntriesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public TimeEntriesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var entries = await _unitOfWork.Repository<TimeEntry>().GetAllAsync(e => e.Task, e => e.Task.Project, e => e.Task.AssignedUser);
        return Ok(entries.Select(e => new
        {
            e.Id,
            User = e.Task?.AssignedUser?.Name,
            Project = e.Task?.Project?.Name,
            Task = e.Task?.Name,
            e.Start,
            e.End,
            e.DurationHours,
            e.ClockifyTimeEntryId
        }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var entry = await _unitOfWork.Repository<TimeEntry>().GetByIdAsync(id);
        if (entry is null) return NotFound();
        return Ok(entry);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTimeEntryRequest request)
    {
        var entry = new TimeEntry
        {
            TaskId = request.TaskId,
            Start = request.Start,
            End = request.End
        };
        await _unitOfWork.Repository<TimeEntry>().AddAsync(entry);
        await _unitOfWork.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entry.Id }, entry);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTimeEntryRequest request)
    {
        var entry = await _unitOfWork.Repository<TimeEntry>().GetByIdAsync(id);
        if (entry is null) return NotFound();

        entry.TaskId = request.TaskId;
        entry.Start = request.Start;
        entry.End = request.End;
        _unitOfWork.Repository<TimeEntry>().Update(entry);
        await _unitOfWork.SaveChangesAsync();
        return Ok(entry);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entry = await _unitOfWork.Repository<TimeEntry>().GetByIdAsync(id);
        if (entry is null) return NotFound();

        _unitOfWork.Repository<TimeEntry>().Remove(entry);
        await _unitOfWork.SaveChangesAsync();
        return NoContent();
    }
}