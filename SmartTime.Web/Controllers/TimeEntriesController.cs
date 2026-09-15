using Microsoft.AspNetCore.Mvc;
using SmartTime.Repository.Interfaces;

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

    // GET /api/timeentries
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var entries = await _unitOfWork.TimeEntries.GetAllAsync(e => e.Task, e => e.Task.Project, e => e.Task.AssignedUser);
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
}