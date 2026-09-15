namespace SmartTime.Services.Configuration;

public class ClockifySettings
{
    public const string SectionName = "Clockify";

    public string ApiKey { get; set; } = string.Empty;
    public string WorkspaceId { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.clockify.me/api/v1";
}