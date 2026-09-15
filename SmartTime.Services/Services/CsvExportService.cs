using SmartTime.Factory;
using SmartTime.Factory.Enums;
using SmartTime.Factory.Interfaces;
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
        var entries = await _unitOfWork.TimeEntries.GetAllAsync();
        var tasks = await _unitOfWork.Tasks.GetAllAsync();
        var users = await _unitOfWork.Users.GetAllAsync();
        var projects = await _unitOfWork.Projects.GetAllAsync();

        var taskById = tasks.ToDictionary(t => t.Id);
        var userById = users.ToDictionary(u => u.Id);
        var projectById = projects.ToDictionary(p => p.Id);

        var rows = entries
            .GroupBy(e => (e.UserId, e.ProjectId, e.TaskId))
            .Select(g =>
            {
                var task = taskById[g.Key.TaskId];
                return new TimeEntryReportRow(
                    User: userById[g.Key.UserId].Name,
                    Project: projectById[g.Key.ProjectId].Name,
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