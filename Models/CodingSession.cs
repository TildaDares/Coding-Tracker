namespace CodingTracker.Models;

public class CodingSession
{
    public int Id { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public string Duration => EndTime.Subtract(StartTime).Duration().ToString("g");
}
