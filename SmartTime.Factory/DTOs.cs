namespace SmartTime.Factory;

public record TimeEntryReportRow(
    string User,
    string Project,
    string Task,
    decimal OriginalEstimateHours,
    double TimeSpentHours);