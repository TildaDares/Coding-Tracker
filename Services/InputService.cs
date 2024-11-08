using CodingTracker.Models;
using Spectre.Console;

namespace CodingTracker.Services;

public static class InputService
{
    public static DateTime GetDateInput(string message, DateTime? minRange = null, DateTime? maxRange = null)
    {
        var validator = new Validator();
        AnsiConsole.Prompt(
            new TextPrompt<string>(message)
                .Validate(dateString =>
                {
                    validator = ValidationService.IsDateValid(dateString, minRange, maxRange);
                    return validator.IsValid ? ValidationResult.Success() : ValidationResult.Error(validator.Message); 
                }));

        return validator.DateTime.Value;
    }
    
    public static void ContinueMenu()
    {
        AnsiConsole.MarkupLine("\n[green]Press any key to continue...[/]");
        Console.ReadLine();
    }

    public static bool ConfirmPrompt(string message)
    {
        var confirmation = AnsiConsole.Prompt(
            new ConfirmationPrompt($"[yellow]{message}[/]"));

        return confirmation;
    }
}