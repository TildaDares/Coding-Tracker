namespace CodingTracker.Models;

public class CodingSessionFilter(DateTime? startTime, DateTime? endTime)
{
    public DateTime? StartTime { get; init; } = startTime;
    public DateTime? EndTime { get; init; } = endTime;
}
