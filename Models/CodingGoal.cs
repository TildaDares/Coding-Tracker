namespace CodingTracker.Models;

public class CodingGoal
{
    public int Id { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public int TotalHoursGoal { get; init; }
}
