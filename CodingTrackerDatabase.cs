using Microsoft.Data.Sqlite;
using System.Configuration;
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
    
    private void CreateCodingTrackerDB()
    {
        using var connection = new SqliteConnection(ConnectionString);
        try
        {
            connection.Open();
            var sql = @" CREATE TABLE IF NOT EXISTS codingTracker (
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