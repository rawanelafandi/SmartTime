using Microsoft.AspNetCore.Mvc;
using SmartTime.Services.Interfaces;

namespace SmartTime.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SyncController : ControllerBase
{
    private readonly ITimeTrackingService _timeTrackingService;

    public SyncController(ITimeTrackingService timeTrackingService)
    {
        _timeTrackingService = timeTrackingService;
    }

    // POST /api/sync/push-sample-data
    [HttpPost("push-sample-data")]
    public async Task<IActionResult> PushSampleData()
    {
        var entriesCreated = await _timeTrackingService.SyncSampleDataAsync();
        return Ok(new { message = "Sample data synced with Clockify and persisted locally.", timeEntriesCreated = entriesCreated });
    }
}