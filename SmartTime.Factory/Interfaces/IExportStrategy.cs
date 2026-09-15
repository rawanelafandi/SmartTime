using SmartTime.Factory.Enums;

namespace SmartTime.Factory.Interfaces;

public interface IExportStrategy
{
    ExportFormat Format { get; }
    byte[] Export(IEnumerable<TimeEntryReportRow> rows);
}