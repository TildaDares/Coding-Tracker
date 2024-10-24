using System.Globalization;

namespace CodingTracker;

public static class ValidationService
{
    public static Validator IsDateValid(string dateString)
    {
        var enUS = new CultureInfo("en-US");
        var dateParse = DateTime.TryParseExact(dateString, "dd/MM/yyyy HH:mm", enUS,
            DateTimeStyles.None, out var dateValue);
        return dateParse ? new Validator(true, dateTime: dateValue) : new Validator(false, "Invalid date format!", dateTime: dateValue);
    }
}