namespace CodingTracker.Models;

public class CodingSession(int id, DateTime startTime, DateTime endTime, int duration)
{
    public int Id { get; private set; } = id;
    public DateTime StartTime { get; private set; } = startTime;
    public DateTime EndTime { get; private set; } = endTime;
    public int Duration { get; private set; } = duration;
}
