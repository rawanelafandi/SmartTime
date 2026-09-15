using SmartTime.Factory;
using SmartTime.Factory.Enums;
using SmartTime.Factory.Interfaces;
using SmartTime.Repository.Entities;
using SmartTime.Repository.Interfaces;
using SmartTime.Services.Interfaces;

namespace SmartTime.Services.Services;

public class CsvExportService : ICsvExportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExportStrategyFactory _exportStrategyFactory;

    public CsvExportService(IUnitOfWork unitOfWork, IExportStrategyFactory exportStrategyFactory)
    {
        _unitOfWork = unitOfWork;
        _exportStrategyFactory = exportStrategyFactory;
    }

    public async Task<byte[]> ExportTimeEntriesCsvAsync()
    {
        var entries = await _unitOfWork.Repository<TimeEntry>().GetAllAsync(e => e.Task, e => e.Task.Project, e => e.Task.AssignedUser);

        var rows = entries
            .GroupBy(e => e.TaskId)
            .Select(g =>
            {
                var task = g.First().Task;
                return new TimeEntryReportRow(
                    User: task.AssignedUser.Name,
                    Project: task.Project.Name,
                    Task: task.Name,
                    OriginalEstimateHours: task.EstimateHours,
                    TimeSpentHours: Math.Round(g.Sum(e => e.DurationHours), 2));
            })
            .OrderBy(r => r.Project).ThenBy(r => r.Task)
            .ToList();

        var strategy = _exportStrategyFactory.GetStrategy(ExportFormat.Csv);
        return strategy.Export(rows);
    }
}