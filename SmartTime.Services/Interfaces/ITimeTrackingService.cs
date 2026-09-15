namespace SmartTime.Services.Interfaces;

public interface ITimeTrackingService
{
    Task<int> SyncSampleDataAsync();
}