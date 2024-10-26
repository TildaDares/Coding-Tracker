using CodingTracker.Enums;
using CodingTracker.Models;
using Spectre.Console;

namespace CodingTracker;

public static class GoalMenu
{
   private static readonly CodingGoalsDatabase _database = new();
   public static void Show()
   {
      Console.Clear();
      var choice = AnsiConsole.Prompt(
         new SelectionPrompt<GoalMenuOptions>()
            .Title("Select an option:")
            .UseConverter(g => $"{g.GetDescription()}")
            .AddChoices(Enum.GetValues<GoalMenuOptions>()));
            
      switch (choice)
      {
         case GoalMenuOptions.InsertCodingGoal:
            InsertCodingGoal();
            break;
         case GoalMenuOptions.GetCodingGoal:
            // GetCodingGoal();
            break;
         case GoalMenuOptions.GetCodingGoals:
            // GetCodingGoals();
            break;
         case GoalMenuOptions.UpdateCodingGoal:
            // UpdateCodingGoal();
            break;
         case GoalMenuOptions.DeleteCodingGoal:
            // DeleteCodingGoal();
            break;
         case GoalMenuOptions.Exit:
            break;
         default:
            break;
      }
   }

   private static void InsertCodingGoal()
   {
      Console.Clear();
      AnsiConsole.MarkupLine("[bold yellow]Your coding goal startTime starts counting when you successfully set a goal[/]");
      AnsiConsole.MarkupLine("[bold yellow]Your coding goal endTime must be set in the future[/]");
      Input.ContinueMenu();
      
      var startTime = DateTime.Now;
      var endTime = Input.GetDateInput(
         "[green]Enter the end date & time of your coding goal in the format[/] [blue]dd/mm/yyyy HH:mm (24-hour format only)[/]:\n", minRange: startTime);
      
      var goalHours = AnsiConsole.Ask<int>("[green]Enter the number of hours of your coding goal:[/]");

      var codingGoal = new CodingGoal() { StartTime = startTime, EndTime = endTime, TotalHoursGoal = goalHours };
      _database.InsertCodingGoal(codingGoal);
      Input.ContinueMenu();
   }
}