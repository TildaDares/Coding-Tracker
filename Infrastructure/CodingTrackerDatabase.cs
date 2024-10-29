using Microsoft.Data.Sqlite;
using System.Configuration;
using CodingTracker.Models;
using Dapper;
using Spectre.Console;

namespace CodingTracker;

public class CodingTrackerDatabase
{
    private readonly string ConnectionString = ConfigurationManager.ConnectionStrings["CodingTrackerDB"].ConnectionString;
    
    public CodingTrackerDatabase()
    {
        CreateCodingTrackerDB();
    }

    public void InsertCodingSession(CodingSession codingSession)
    {
        using var connection = new SqliteConnection(this.ConnectionString);
        try
        {
            connection.Open();
            const string sql = "INSERT INTO codingTracker(startTime, endTime, duration) VALUES (@StartTime, @EndTime, @Duration)";
            var rowsAffected = connection.Execute(sql, codingSession);
            AnsiConsole.MarkupLine($"[green]{rowsAffected} row(s) inserted.[/]");
        }
        catch (SqliteException e)
        {
            AnsiConsole.MarkupLine($"[red]Unable to insert into database. {e.Message}[/]");
        }
        finally
        {
            connection.Close();
        }
    }

    public CodingSession GetCodingSession(int id)
    {
        using var connection = new SqliteConnection(this.ConnectionString);
        CodingSession session = null;
        try
        {
            connection.Open();
            const string sql = "SELECT * FROM codingTracker WHERE id = @Id";
            session = connection.QuerySingleOrDefault<CodingSession>(sql, new {Id = id});
        }
        catch (SqliteException e)
        {
            AnsiConsole.MarkupLine($"[red]Unable to retrieve coding session record with ID: {id}. {e.Message}[/]");
        }
        finally
        {
            connection.Close();
        }
        
        return session;
    }

    public double GetSumOfCodingSessionDuration(DateTime? startTime, DateTime? endTime)
    {
        using var connection = new SqliteConnection(this.ConnectionString);
        var sum = 0.0;
        try
        {
            connection.Open();
            var sql = "SELECT printf('%.2f', SUM(duration)) FROM codingTracker";
            if (startTime.HasValue && endTime.HasValue)
            {
                sql += " WHERE DATETIME(startTime) >= DATETIME(@StartTime) AND DATETIME(endTime) <= DATETIME(@EndTime)";
            }
            
            sum = connection.ExecuteScalar<double>(sql, new { StartTime = startTime.Value, EndTime = endTime.Value });
        }
        catch (SqliteException e)
        {
            AnsiConsole.MarkupLine($"[red]Unable to retrieve all coding session records. {e.Message}[/]");
        }
        finally
        {
            connection.Close();
        }
        
        return sum;
    }
    
    public List<CodingSession> GetAllCodingSessions()
    {
        using var connection = new SqliteConnection(this.ConnectionString);
        var sessions = new List<CodingSession>();
        try
        {
            connection.Open();
            const string sql = "SELECT * FROM codingTracker";
            sessions = connection.Query<CodingSession>(sql).ToList();
        }
        catch (SqliteException e)
        {
            AnsiConsole.MarkupLine($"[red]Unable to retrieve all coding session records. {e.Message}[/]");
        }
        finally
        {
            connection.Close();
        }
        
        return sessions;
    }

    public void UpdateCodingSession(CodingSession codingSession)
    {
        using var connection = new SqliteConnection(this.ConnectionString);
        CodingSession session = null;
        try
        {
            connection.Open();
            const string sql = "UPDATE codingTracker SET startTime = @StartTime, endTime = @EndTime, duration = @Duration WHERE id = @Id";
            var rowsAffected = connection.Execute(sql, codingSession);
            AnsiConsole.MarkupLine($"[green]{rowsAffected} row(s) updated.[/]");
        }
        catch (SqliteException e)
        {
            AnsiConsole.MarkupLine($"[red]Unable to update coding session record with ID: {codingSession.Id}. {e.Message}[/]");
        }
        finally
        {
            connection.Close();
        }
    }

    public void DeleteCodingSession(int id)
    {
        using var connection = new SqliteConnection(this.ConnectionString);
        CodingSession session = null;
        try
        {
            connection.Open();
            const string sql = "DELETE FROM codingTracker WHERE id = @Id";
            var rowsAffected = connection.Execute(sql, new {@Id = id});
            AnsiConsole.MarkupLine($"[green]{rowsAffected} row(s) deleted.[/]");
        }
        catch (SqliteException e)
        {
            AnsiConsole.MarkupLine($"[red]Unable to delete coding session record with ID: {id}. {e.Message}[/]");
        }
        finally
        {
            connection.Close();
        }
    }
    
    public long CountCodingSessions()
    {
        using var connection = new SqliteConnection(this.ConnectionString);
        var count = 0;
        try
        {
            connection.Open();
            const string sql = "SELECT COUNT(*) FROM codingTracker";
            count = connection.ExecuteScalar<int>(sql);
        } 
        catch (SqliteException e)
        {
            AnsiConsole.MarkupLine($"[red]Unable to count coding session records. {e.Message}[/]");
        }
        finally
        {
            connection.Close();
        }

        return count;
    }
    
    private void CreateCodingTrackerDB()
    {
        using var connection = new SqliteConnection(ConnectionString);
        try
        {
            connection.Open();
            const string sql = @" CREATE TABLE IF NOT EXISTS codingTracker (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                startTime TEXT NOT NULL,
                endTime Text NOT NULL
                duration TEXT NOT NULL DEFAULT '0.00')";
            connection.Execute(sql);
        }
        catch (SqliteException e)
        {
            AnsiConsole.MarkupLine($"[red]Unable to create coding tracker database. {e.Message}[/]");
        }
        finally
        {
            connection.Close();
        }
    }
}