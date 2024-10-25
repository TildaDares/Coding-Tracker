namespace CodingTracker.Models;

public class CodingSession
{
    public int Id { get; init; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Duration => EndTime.Subtract(StartTime).Duration().ToString("g");
}
