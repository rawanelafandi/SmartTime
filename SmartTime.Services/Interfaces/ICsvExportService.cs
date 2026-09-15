namespace SmartTime.Services.Interfaces;

public interface ICsvExportService
{
    Task<byte[]> ExportTimeEntriesCsvAsync();
}