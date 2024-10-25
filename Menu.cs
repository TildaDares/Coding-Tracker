using CodingTracker.Enums;
using CodingTracker.Models;
using Spectre.Console;

namespace CodingTracker;

public class Menu
{
    private readonly CodingTrackerDatabase _database = new();
    public void DisplayMenu()
    {
        var exit = false;
        while (!exit)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[b][green]Welcome to Coding Tracker![/][/]\n");
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<MenuOptions>()
                    .Title("Select an option:")
                    .UseConverter(o => $"{o.GetDescription()}")
                    .AddChoices(Enum.GetValues<MenuOptions>()));
            
            switch (choice)
            {
                case MenuOptions.InsertCodingSession:
                    InsertCodingLog();
                    break;
                // case "2":
                //     GetHabit();
                //     break;
                // case "3":
                //     GetHabits();
                //     break;
                // case "4":
                //     UpdateHabit();
                //     break;
                // case "5":
                //     DeleteHabit();
                //     break;
                // case "6":
                //     ViewHabitReportByTypeAndDate();
                //     break;
                // case "7":
                //     ViewHabitsByDate();
                //     break;
                default:
                    exit = true;
                    break;
            }
        }
    }

    private void InsertCodingLog()
    {
        Console.Clear();
        var startTime = GetDateInput(
            "[green]Enter the start date & time of your coding log in the format[/] [blue]dd/mm/yyyy HH:mm (24-hour format only)[/]:\n");
        
        Console.Clear();
        var endTime = GetDateInput(
            "[green]Enter the end date & time of your coding log in the format[/] [blue]dd/mm/yyyy HH:mm (24-hour format only)[/]:\n", minRange: startTime);
        Console.Clear();

        var codingSession = new CodingSession() { StartTime = startTime, EndTime = endTime };
        _database.InsertCodingSession(codingSession);
        ContinueMenu();
    }

    private DateTime GetDateInput(string message, DateTime? minRange = null, DateTime? maxRange = null)
    {
        Console.Clear();
        var validator = new Validator();
        AnsiConsole.Prompt(
            new TextPrompt<string>(message)
                .Validate((dateString) =>
                {
                    validator = ValidationService.IsDateValid(dateString, minRange, maxRange);
                    return validator.IsValid ? ValidationResult.Success() : ValidationResult.Error(validator.Message); 
                }));

        return validator.DateTime.Value;
    }
    
    private void ContinueMenu()
    {
        AnsiConsole.MarkupLine("\n[green]Press any key to continue...[/]");
        Console.ReadLine();
    }
}