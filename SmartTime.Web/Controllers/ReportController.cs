using Microsoft.AspNetCore.Mvc;
using SmartTime.Services.Interfaces;

namespace SmartTime.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ICsvExportService _csvExportService;

    public ReportsController(ICsvExportService csvExportService)
    {
        _csvExportService = csvExportService;
    }

    // GET /api/reports/time-entries.csv
    [HttpGet("time-entries.csv")]
    public async Task<IActionResult> ExportTimeEntries()
    {
        var csvBytes = await _csvExportService.ExportTimeEntriesCsvAsync();
        return File(csvBytes, "text/csv", "time-entries-report.csv");
    }
}