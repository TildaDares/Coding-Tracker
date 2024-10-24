using System.Globalization;

namespace CodingTracker;

public static class ValidationService
{
    public static Validator IsDateValid(string dateString, DateTime? minRange = null, DateTime? maxRange = null)
    {
        minRange ??= DateTime.MinValue;
        maxRange ??= DateTime.MaxValue;
        var enUS = new CultureInfo("en-US");
        var isValid = DateTime.TryParseExact(dateString, "dd/MM/yyyy HH:mm", enUS,
            DateTimeStyles.None, out var dateValue);
        var isWithinRange = dateValue >= minRange && dateValue <= maxRange;

        var message = "";
        if (!isValid) message = "Invalid format";
        else if (!isWithinRange) message = "Date is out of range!";
        
        return new Validator(isValid && isWithinRange, message, dateTime: dateValue);
    }
}
