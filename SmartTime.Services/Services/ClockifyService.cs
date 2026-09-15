using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SmartTime.Services.Configuration;
using SmartTime.Services.DTOs;
using SmartTime.Services.Interfaces;

namespace SmartTime.Services.Services;

public class ClockifyService : IClockifyService
{
    private readonly HttpClient _http;
    private readonly ClockifySettings _settings;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ClockifyService(HttpClient http, IOptions<ClockifySettings> settings)
    {
        _settings = settings.Value;
        _http = http;
        _http.BaseAddress = new Uri(_settings.BaseUrl);
        _http.DefaultRequestHeaders.Add("X-Api-Key", _settings.ApiKey);
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<string> GetCurrentUserIdAsync()
    {
    var body = await _http.GetStringAsync("user");
    var user = JsonSerializer.Deserialize<ClockifyUserDto>(body, JsonOptions);
    return user!.Id;
    }
    
    public async Task<string?> FindUserIdByEmailAsync(string email)
    {
        var body = await _http.GetStringAsync($"workspaces/{_settings.WorkspaceId}/users");
        var users = ParseList<ClockifyUserDto>(body);

        return users.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase))?.Id;
    }

    public async Task<string> EnsureProjectAsync(string projectName)
    {
        var listBody = await _http.GetStringAsync(
            $"workspaces/{_settings.WorkspaceId}/projects?page-size=200");
        var existing = ParseList<ClockifyProjectDto>(listBody);

        var match = existing.FirstOrDefault(p =>
            string.Equals(p.Name, projectName, StringComparison.OrdinalIgnoreCase));
        if (match is not null)
        {
            return match.Id;
        }

        var response = await _http.PostAsJsonAsync(
            $"workspaces/{_settings.WorkspaceId}/projects",
            new CreateProjectRequest(projectName));

        var createBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Clockify POST project failed ({(int)response.StatusCode}): {createBody}");
        }

        var created = JsonSerializer.Deserialize<ClockifyProjectDto>(createBody, JsonOptions);
        return created!.Id;
    }

    public async Task<string> CreateTaskAsync(string clockifyProjectId, string taskName, decimal estimateHours, string? assigneeClockifyUserId)
    {
        var listBody = await _http.GetStringAsync(
            $"workspaces/{_settings.WorkspaceId}/projects/{clockifyProjectId}/tasks?page-size=200");
        var existing = ParseList<ClockifyTaskDto>(listBody);

        var match = existing.FirstOrDefault(t =>
            string.Equals(t.Name, taskName, StringComparison.OrdinalIgnoreCase));
        if (match is not null)
        {
            return match.Id;
        }

        var request = new CreateTaskRequest(
            Name: taskName,
            Estimate: ToIso8601Duration(estimateHours),
            AssigneeIds: assigneeClockifyUserId is null ? new List<string>() : new List<string> { assigneeClockifyUserId });

        var response = await _http.PostAsJsonAsync(
            $"workspaces/{_settings.WorkspaceId}/projects/{clockifyProjectId}/tasks",
            request);

        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Clockify POST task failed ({(int)response.StatusCode}): {body}");
        }

        var created = JsonSerializer.Deserialize<ClockifyTaskDto>(body, JsonOptions);
        return created!.Id;
    }

    public async Task<string> CreateTimeEntryAsync(string clockifyUserId, string clockifyProjectId, string clockifyTaskId, DateTime start, DateTime end, string description)
    {
        var request = new CreateTimeEntryRequest(
            Start: start.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
            End: end.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
            ProjectId: clockifyProjectId,
            TaskId: clockifyTaskId,
            Description: description);

        var response = await _http.PostAsJsonAsync(
            $"workspaces/{_settings.WorkspaceId}/user/{clockifyUserId}/time-entries",
            request);

        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Clockify POST time entry failed ({(int)response.StatusCode}): {body}");
        }

        var created = JsonSerializer.Deserialize<ClockifyTimeEntryDto>(body, JsonOptions);
        return created!.Id;
    }

    private static List<T> ParseList<T>(string json)
    {
        var trimmed = json.TrimStart();
        if (trimmed.StartsWith('['))
        {
            return JsonSerializer.Deserialize<List<T>>(json, JsonOptions) ?? new List<T>();
        }

        var wrapped = JsonSerializer.Deserialize<ClockifyPagedResult<T>>(json, JsonOptions);
        return wrapped?.Value ?? new List<T>();
    }

    private static string ToIso8601Duration(decimal hours)
    {
        var totalMinutes = (int)Math.Round(hours * 60, MidpointRounding.AwayFromZero);
        var h = totalMinutes / 60;
        var m = totalMinutes % 60;
        return $"PT{h}H{m}M";
    }
}