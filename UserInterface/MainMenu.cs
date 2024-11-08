using System.Configuration;
using CodingTracker.Enums.Models;
using CodingTracker.Enums.Services;
using CodingTracker.Infrastructure;
using CodingTracker.Models;
using CodingTracker.Services;
using Spectre.Console;

namespace CodingTracker.UserInterface;

public class MainMenu
{
    private readonly CodingTrackerDatabase _database;
    private readonly GoalMenu _goalMenu;
    private static readonly string? DateFormat = ConfigurationManager.AppSettings["dateFormat"];
    
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
                case MainMenuOptions.FilterCodingSessionsByPeriod:
                    FilterCodingSessionsByPeriod();
                    break;
                case MainMenuOptions.ViewCodingSessionReportByPeriod:
                    ViewCodingSessionReportByPeriod();
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
        var startTime = InputService.GetDateInput(
            $"[green]Enter the start date & time of your coding log in the format[/] [blue]{DateFormat} (24-hour format only)[/]:\n");
        
        var endTime = InputService.GetDateInput(
            $"[green]Enter the end date & time of your coding log in the format[/] [blue]{DateFormat} (24-hour format only)[/]:\n", minRange: startTime);

        var confirmation = InputService.ConfirmPrompt("[yellow]Save coding session to database?[/]");
        if (!confirmation) return;
        
        var codingSession = new CodingSession(startTime, endTime);
        var rowsAffected = _database.InsertCodingSession(codingSession);
        AnsiConsole.MarkupLine($"[green]{rowsAffected} row(s) inserted.[/]");
        InputService.ContinueMenu();
    }

    private void GetCodingSession()
    {
        Console.Clear();
        if (!HasCodingSessions())
        {
            InputService.ContinueMenu();
            return;
        }
    
        GetCodingSessions();
        var id = AnsiConsole.Ask<int>("Enter the coding session ID you wish to retrieve:");
        var session = _database.GetCodingSession(id);

        if (session == null)
        {
            AnsiConsole.MarkupLine("[red]No coding session found with that ID![/]");
            InputService.ContinueMenu();
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
        InputService.ContinueMenu();
    }
    
    private void GetCodingSessions()
    {
        Console.Clear();
        var sessions = _database.GetAllCodingSessions();
        if (sessions.Count == 0)
        {
            AnsiConsole.MarkupLine("[green]No coding sessions found![/]");
            InputService.ContinueMenu();
            return;
        }

        DisplayCodingSessions(sessions, "All coding sessions:");
        InputService.ContinueMenu();
    }

    private void UpdateCodingSession()
    {
        Console.Clear();
        if (!HasCodingSessions())
        {
            InputService.ContinueMenu();
            return;
        }
    
        GetCodingSessions();
        var id = AnsiConsole.Ask<int>("Enter the coding session ID you wish to update:");
        var codingSession = _database.GetCodingSession(id);
        
        if (codingSession == null)
        {
            AnsiConsole.MarkupLine("[red]No coding session found with that ID![/]");
            InputService.ContinueMenu();
            return;
        }
        
        var startTime = InputService.GetDateInput(
            $"[green]Enter the updated start date & time of your coding log in the format[/] [blue]{DateFormat} (24-hour format only)[/]:\n");
        
        var endTime = InputService.GetDateInput(
            $"[green]Enter the updated end date & time of your coding log in the format[/] [blue]{DateFormat} (24-hour format only)[/]:\n", minRange: startTime);
        
        var confirmation = InputService.ConfirmPrompt("[yellow]Save updated coding session to database?[/]");
        if (!confirmation) return;
        
        var updatedSession = new CodingSession(startTime, endTime)
        {
            Id = id
        };
        var rowsAffected = _database.UpdateCodingSession(updatedSession);
        AnsiConsole.MarkupLine($"[green]{rowsAffected} row(s) updated.[/]");
        InputService.ContinueMenu();
    }

    private void DeleteCodingSession()
    {
        Console.Clear();
        if (!HasCodingSessions())
        {
            InputService.ContinueMenu();
            return;
        }
    
        GetCodingSessions();
        var id = AnsiConsole.Ask<int>("Enter the coding session ID you wish to delete:");
        var confirmation = InputService.ConfirmPrompt("[yellow]This action is irreversible. Confirm delete?[/]");
        if (!confirmation) return;
        
        var rowsAffected = _database.DeleteCodingSession(id);
        AnsiConsole.MarkupLine($"[green]{rowsAffected} row(s) deleted.[/]");
        InputService.ContinueMenu();
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
        
        var confirmation = InputService.ConfirmPrompt("[yellow]Save coding session to database?[/]");
        if (!confirmation)
        {
            InputService.ContinueMenu();
            return;
        }

        Console.Clear();
        var codingSession = new CodingSession(stopwatchService.StartTime, stopwatchService.EndTime);
        _database.InsertCodingSession(codingSession);
        InputService.ContinueMenu();
    }

    private void FilterCodingSessionsByPeriod()
    {
        Console.Clear();
        if (!HasCodingSessions())
        {
            InputService.ContinueMenu();
            return;
        }
        
        var startTime = InputService.GetDateInput(
            $"[green]Enter the start date & time of sessions you want to filter in the format[/] [blue]{DateFormat} (24-hour format only)[/]:\n");
        Console.Clear();
        
        var endTime = InputService.GetDateInput(
            $"[green]Enter the end date & time of sessions you want to filter in the format[/] [blue]{DateFormat} (24-hour format only)[/]:\n", minRange: startTime);
        Console.Clear();

        var filter = new CodingSessionFilter{ StartTime = startTime, EndTime = endTime };
        var sessions = _database.GetAllCodingSessions(filter);
        
        if (sessions.Count == 0)
        {
            AnsiConsole.MarkupLine("[green]No coding sessions found![/]");
            InputService.ContinueMenu();
            return;
        }
        
        DisplayCodingSessions(sessions,$"All coding sessions in the period {startTime} to {endTime}:");
        InputService.ContinueMenu();
    }

    private void ViewCodingSessionReportByPeriod()
    {
        Console.Clear();
        if (!HasCodingSessions())
        {
            InputService.ContinueMenu();
            return;
        }
        
        var startTime = InputService.GetDateInput(
            $"[green]Enter the start date & time of sessions you want to view a report for in the format[/] [blue]{DateFormat} (24-hour format only)[/]:\n");
        Console.Clear();
        
        var endTime = InputService.GetDateInput(
            $"[green]Enter the end date & time of sessions you want to view a report for in the format[/] [blue]{DateFormat} (24-hour format only)[/]:\n", minRange: startTime);
        Console.Clear();
        
        var filter = new CodingSessionFilter{ StartTime = startTime, EndTime = endTime };
        var stats = _database.GetSumOfCodingSessionDuration(filter);
        var averageHours = stats.TotalHours / stats.RecordCount;

        var totalHoursText = $"Total hours of coding sessions: [yellow]{stats.TotalHours:F}[/]";
        var averageHoursText = $"Average hours of coding sessions: [yellow]{averageHours:F}[/]";
        
        var panel = new Panel($"[blue]{totalHoursText}\n{averageHoursText}[/]")
        {
            Border = BoxBorder.Double,
            Expand = true,
            Header = new PanelHeader($"Report for the the coding sessions in the period {startTime} to {endTime}:")
        };

        AnsiConsole.Write(panel);
        InputService.ContinueMenu();
    }
    
    private bool HasCodingSessions()
    {
        var count = _database.CountCodingSessions();
        if (count >= 1) return true;
        AnsiConsole.MarkupLine("[green]No coding sessions found![/]");
        return false;
    }

    private static void BuildTableHeader(Table table)
    {
        table.AddColumn(new TableColumn("[yellow]Id[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]StartTime[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]EndTime[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]Duration (hrs)[/]").Centered());
    }
    
    private static void BuildTableRows(Table table, CodingSession codingSession)
    {
        table.AddRow($"[blue]{codingSession.Id}[/]", $"[blue]{codingSession.StartTime}[/]", $"[blue]{codingSession.EndTime}[/]", $"[blue]{codingSession.Duration}[/]");
    }

    private static void DisplayCodingSessions(List<CodingSession> sessions, string title)
    {
        var panel = new Panel(title)
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
    }
}
