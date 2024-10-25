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
            const string sql = "INSERT INTO codingTracker(startTime, endTime) VALUES (@StartTime, @EndTime)";
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

    public CodingSession GetCodingSession(CodingSession codingSession)
    {
        using var connection = new SqliteConnection(this.ConnectionString);
        CodingSession session = null;
        try
        {
            connection.Open();
            const string sql = "SELECT * FROM codingTracker WHERE id = @Id";
            session = connection.QuerySingleOrDefault<CodingSession>(sql, codingSession);
        }
        catch (SqliteException e)
        {
            AnsiConsole.MarkupLine($"[red]Unable to retrieve coding session record with ID: {codingSession.Id}. {e.Message}[/]");
        }
        finally
        {
            connection.Close();
        }
        
        return session;
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
            const string sql = "UPDATE codingTracker SET startTime = @StartTime, endTime = @EndTime WHERE id = @Id";
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

    public void DeleteCodingSession(CodingSession codingSession)
    {
        using var connection = new SqliteConnection(this.ConnectionString);
        CodingSession session = null;
        try
        {
            connection.Open();
            const string sql = "DELETE FROM codingTracker WHERE id = @Id";
            var rowsAffected = connection.Execute(sql, codingSession);
            AnsiConsole.MarkupLine($"[green]{rowsAffected} row(s) deleted.[/]");
        }
        catch (SqliteException e)
        {
            AnsiConsole.MarkupLine($"[red]Unable to delete coding session record with ID: {codingSession.Id}. {e.Message}[/]");
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
                endTime Text NOT NULL )";
            connection.Execute(sql);
        }
        catch (SqliteException e)
        {
            AnsiConsole.MarkupLine($"[red]Unable to create database. {e.Message}[/]");
        }
        finally
        {
            connection.Close();
        }
    }
}