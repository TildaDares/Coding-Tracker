namespace CodingTracker.Models;

public class CodingSession
{
    public int Id { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public int Duration { get; init; }
}
