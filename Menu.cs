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
                    InsertCodingSession();
                    break;
                case MenuOptions.GetCodingSession:
                    GetCodingSession();
                    break;
                case MenuOptions.GetCodingSessions:
                    GetCodingSessions();
                    break;
                case MenuOptions.UpdateCodingSession:
                    UpdateCodingSession();
                    break;
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

    private void InsertCodingSession()
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

    private void GetCodingSession()
    {
        Console.Clear();
        if (!HasCodingSessions())
        {
            ContinueMenu();
            return;
        }
    
        GetCodingSessions();
        var id = AnsiConsole.Ask<int>("Enter the coding session ID you wish to retrieve:");
        var codingSession = new CodingSession() { Id = id };
        var session = _database.GetCodingSession(codingSession);

        if (session == null)
        {
            AnsiConsole.MarkupLine("[red]No coding session found with that ID![/]");
            ContinueMenu();
            return;
        }
    
        Console.Clear();
        var panel = new Panel("Coding Session Record:")
        {
            Border = BoxBorder.Ascii
        };
        
        var table = new Table();
        BuildTableHeader(table);
        BuildTableRows(table, session);
        AnsiConsole.Write(panel);
        AnsiConsole.Write(table);
        ContinueMenu();
    }
    
    private void GetCodingSessions()
    {
        Console.Clear();
        var sessions = _database.GetAllCodingSessions();
        if (sessions.Count == 0)
        {
            AnsiConsole.MarkupLine("[green]No coding sessions found![/]");
            ContinueMenu();
            return;
        }
        
        var panel = new Panel("All Coding Sessions records:")
        {
            Border = BoxBorder.Ascii
        };
        var table = new Table();
        BuildTableHeader(table);
    
        foreach (var session in sessions)
        {
            BuildTableRows(table, session);
        }
        
        AnsiConsole.Write(panel);
        AnsiConsole.Write(table);
        ContinueMenu();
    }

    private void UpdateCodingSession()
    {
        Console.Clear();
        if (!HasCodingSessions())
        {
            ContinueMenu();
            return;
        }
    
        GetCodingSessions();
        var id = AnsiConsole.Ask<int>("Enter the coding session ID you wish to update:");
        var codingSession = new CodingSession() { Id = id };
        var session = _database.GetCodingSession(codingSession);
        
        if (session == null)
        {
            AnsiConsole.MarkupLine("[red]No coding session found with that ID![/]");
            ContinueMenu();
            return;
        }
        
        var startTime = GetDateInput(
            "[green]Enter the updated start date & time of your coding log in the format[/] [blue]dd/mm/yyyy HH:mm (24-hour format only)[/]:\n");
        Console.Clear();
        
        var endTime = GetDateInput(
            "[green]Enter the updated end date & time of your coding log in the format[/] [blue]dd/mm/yyyy HH:mm (24-hour format only)[/]:\n", minRange: startTime);
        Console.Clear();

        codingSession.StartTime = startTime;
        codingSession.EndTime = endTime;
        _database.UpdateCodingSession(codingSession);
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
    
    private bool HasCodingSessions()
    {
        var count = _database.CountCodingSessions();
        if (count >= 1) return true;
        AnsiConsole.MarkupLine("[green]No coding sessions found![/]");
        return false;
    }
    
    private void ContinueMenu()
    {
        AnsiConsole.MarkupLine("\n[green]Press any key to continue...[/]");
        Console.ReadLine();
    }

    private void BuildTableHeader(Table table)
    {
        table.AddColumn(new TableColumn("[yellow]Id[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]StartTime[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]EndTime[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Duration (Days:Hrs:Mins:Secs)[/]").Centered());
    }
    
    private void BuildTableRows(Table table, CodingSession codingSession)
    {
        table.AddRow($"[blue]{codingSession.Id}[/]", $"[blue]{codingSession.StartTime}[/]", $"[blue]{codingSession.EndTime}[/]", $"[blue]{codingSession.Duration}[/]");
    }
}