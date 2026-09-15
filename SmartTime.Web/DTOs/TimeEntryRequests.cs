namespace SmartTime.Web.DTOs;

public record CreateTimeEntryRequest(int TaskId, DateTime Start, DateTime End);
public record UpdateTimeEntryRequest(int TaskId, DateTime Start, DateTime End);