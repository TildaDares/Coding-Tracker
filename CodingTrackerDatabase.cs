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
    
    public List<CodingSession> GetAllCodingSessions()
    {
        using var connection = new SqliteConnection(this.ConnectionString);
        var sessions = new List<CodingSession>();
        try
        {
            connection.Open();
            const string sql = "SELECT * FROM codingTracker";
            sessions = connection.Query<CodingSession>(sql).ToList();
            return sessions;
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