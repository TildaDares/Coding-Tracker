namespace CodingTracker.Services;

public static class SeederService
{
    public static DateTime GetRandomDateTime(DateTime? startTime = null)
    {
        if (startTime == null)
        {
            var twoYearsAgo = DateTime.Now.AddYears(-2).Year;
            startTime = new DateTime(twoYearsAgo, 1, 1);
        }
        
        var range = DateTime.Now.Subtract(startTime.Value);
        var days = Random.Shared.Next(0, range.Days);
        return startTime.Value.AddDays(days);
    }
}
