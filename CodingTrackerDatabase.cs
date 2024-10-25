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