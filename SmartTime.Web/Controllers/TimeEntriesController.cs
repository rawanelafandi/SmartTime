using Microsoft.AspNetCore.Mvc;
using SmartTime.Services.Interfaces;
using SmartTime.Web.DTOs;

namespace SmartTime.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeEntriesController : ControllerBase
{
    private readonly ITimeEntryService _timeEntryService;

    public TimeEntriesController(ITimeEntryService timeEntryService)
    {
        _timeEntryService = timeEntryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var entries = await _timeEntryService.GetAllAsync();
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
        var entry = await _timeEntryService.GetByIdAsync(id);
        if (entry is null) return NotFound();
        return Ok(entry);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTimeEntryRequest request)
    {
        var entry = await _timeEntryService.CreateAsync(request.TaskId, request.Start, request.End);
        return CreatedAtAction(nameof(GetById), new { id = entry.Id }, entry);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTimeEntryRequest request)
    {
        var entry = await _timeEntryService.UpdateAsync(id, request.TaskId, request.Start, request.End);
        if (entry is null) return NotFound();
        return Ok(entry);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _timeEntryService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}