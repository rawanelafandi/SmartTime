using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using SmartTime.Factory.Enums;
using SmartTime.Factory.Interfaces;

namespace SmartTime.Factory.Strategies;

public class CsvExportStrategy : IExportStrategy
{
    public ExportFormat Format => ExportFormat.Csv;

    public byte[] Export(IEnumerable<TimeEntryReportRow> rows)
    {
        using var memoryStream = new MemoryStream();
        using (var writer = new StreamWriter(memoryStream, leaveOpen: true))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            csv.Context.RegisterClassMap<TimeEntryReportRowMap>();
            csv.WriteRecords(rows);
        }

        return memoryStream.ToArray();
    }

    private sealed class TimeEntryReportRowMap : ClassMap<TimeEntryReportRow>
    {
        public TimeEntryReportRowMap()
        {
            Map(r => r.User).Name("User");
            Map(r => r.Project).Name("Project");
            Map(r => r.Task).Name("Task");
            Map(r => r.OriginalEstimateHours).Name("OriginalEstimate (hrs)").TypeConverterOption.Format("0.##");
            Map(r => r.TimeSpentHours).Name("TimeSpent (hrs)").TypeConverterOption.Format("0.00");
        }
    }
}