using CodingTracker.Enums;
using CodingTracker.Models;
using Spectre.Console;

namespace CodingTracker;

public class MainMenu
{
    private readonly CodingTrackerDatabase _database;
    private readonly GoalMenu _goalMenu;
    
    public MainMenu()
    {
        _database = new CodingTrackerDatabase();
        _goalMenu = new GoalMenu(_database);
    }
    
    public void DisplayMenu()
    {
        var exit = false;
        while (!exit)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[b][green]Welcome to Coding Tracker![/][/]\n");
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<MainMenuOptions>()
                    .Title("Select an option:")
                    .UseConverter(o => $"{o.GetDescription()}")
                    .AddChoices(Enum.GetValues<MainMenuOptions>()));
            
            switch (choice)
            {
                case MainMenuOptions.InsertCodingSession:
                    InsertCodingSession();
                    break;
                case MainMenuOptions.GetCodingSession:
                    GetCodingSession();
                    break;
                case MainMenuOptions.GetCodingSessions:
                    GetCodingSessions();
                    break;
                case MainMenuOptions.UpdateCodingSession:
                    UpdateCodingSession();
                    break;
                case MainMenuOptions.DeleteCodingSession:
                    DeleteCodingSession();
                    break;
                case MainMenuOptions.StartCodingSession:
                    StartCodingSession();
                    break;
                case MainMenuOptions.Goals:
                    _goalMenu.Show();
                    break;
                case MainMenuOptions.Exit:
                default:
                    exit = true;
                    break;
            }
        }
    }

    private void InsertCodingSession()
    {
        Console.Clear();
        var startTime = Input.GetDateInput(
            "[green]Enter the start date & time of your coding log in the format[/] [blue]dd/mm/yyyy HH:mm (24-hour format only)[/]:\n");
        
        Console.Clear();
        var endTime = Input.GetDateInput(
            "[green]Enter the end date & time of your coding log in the format[/] [blue]dd/mm/yyyy HH:mm (24-hour format only)[/]:\n", minRange: startTime);
        Console.Clear();

        var codingSession = new CodingSession(startTime, endTime);
        _database.InsertCodingSession(codingSession);
        Input.ContinueMenu();
    }

    private void GetCodingSession()
    {
        Console.Clear();
        if (!HasCodingSessions())
        {
            Input.ContinueMenu();
            return;
        }
    
        GetCodingSessions();
        var id = AnsiConsole.Ask<int>("Enter the coding session ID you wish to retrieve:");
        var session = _database.GetCodingSession(id);

        if (session == null)
        {
            AnsiConsole.MarkupLine("[red]No coding session found with that ID![/]");
            Input.ContinueMenu();
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
        Input.ContinueMenu();
    }
    
    private void GetCodingSessions()
    {
        Console.Clear();
        var sessions = _database.GetAllCodingSessions();
        if (sessions.Count == 0)
        {
            AnsiConsole.MarkupLine("[green]No coding sessions found![/]");
            Input.ContinueMenu();
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
        Input.ContinueMenu();
    }

    private void UpdateCodingSession()
    {
        Console.Clear();
        if (!HasCodingSessions())
        {
            Input.ContinueMenu();
            return;
        }
    
        GetCodingSessions();
        var id = AnsiConsole.Ask<int>("Enter the coding session ID you wish to update:");
        var codingSession = _database.GetCodingSession(id);
        
        if (codingSession == null)
        {
            AnsiConsole.MarkupLine("[red]No coding session found with that ID![/]");
            Input.ContinueMenu();
            return;
        }
        
        var startTime = Input.GetDateInput(
            "[green]Enter the updated start date & time of your coding log in the format[/] [blue]dd/mm/yyyy HH:mm (24-hour format only)[/]:\n");
        Console.Clear();
        
        var endTime = Input.GetDateInput(
            "[green]Enter the updated end date & time of your coding log in the format[/] [blue]dd/mm/yyyy HH:mm (24-hour format only)[/]:\n", minRange: startTime);
        Console.Clear();
        
        var updatedSession = new CodingSession(startTime, endTime)
        {
            Id = id
        };
        _database.UpdateCodingSession(updatedSession);
        Input.ContinueMenu();
    }

    private void DeleteCodingSession()
    {
        Console.Clear();
        if (!HasCodingSessions())
        {
            Input.ContinueMenu();
            return;
        }
    
        GetCodingSessions();
        var id = AnsiConsole.Ask<int>("Enter the coding session ID you wish to delete:");
        _database.DeleteCodingSession(id);
        Input.ContinueMenu();
    }

    private void StartCodingSession()
    {
        Console.Clear();
        var stopwatchService = new StopwatchService();
        AnsiConsole.MarkupLine("[green]Press any key to stop the coding session...[/]");
        stopwatchService.Start();
        Console.ReadKey();
        stopwatchService.Stop();
        AnsiConsole.MarkupLine($"[green]You ran the coding session for: {stopwatchService.Duration}...[/]");
        
        var confirmation = AnsiConsole.Prompt(
            new ConfirmationPrompt("[yellow]Save coding session to database?[/]"));
        if (!confirmation)
        {
            Input.ContinueMenu();
            return;
        };
        
        Console.Clear();
        var codingSession = new CodingSession(stopwatchService.StartTime, stopwatchService.EndTime);
        _database.InsertCodingSession(codingSession);
        Input.ContinueMenu();
    }
    
    private bool HasCodingSessions()
    {
        var count = _database.CountCodingSessions();
        if (count >= 1) return true;
        AnsiConsole.MarkupLine("[green]No coding sessions found![/]");
        return false;
    }

    private void BuildTableHeader(Table table)
    {
        table.AddColumn(new TableColumn("[yellow]Id[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]StartTime[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]EndTime[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Duration (hrs)[/]").Centered());
    }
    
    private void BuildTableRows(Table table, CodingSession codingSession)
    {
        table.AddRow($"[blue]{codingSession.Id}[/]", $"[blue]{codingSession.StartTime}[/]", $"[blue]{codingSession.EndTime}[/]", $"[blue]{codingSession.Duration}[/]");
    }
}