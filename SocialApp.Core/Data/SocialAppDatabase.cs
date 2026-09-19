using Microsoft.Data.Sqlite;

namespace SocialApp.Core.Data;

// SQLite holbolt 
// Repository classuud ene deereesee holbolt avch ajillana.
public class SocialAppDatabase
{
    private readonly string _connectionString;

    public SocialAppDatabase(string databaseFilePath)
    {
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = databaseFilePath
        }.ToString();
    }

    public SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    public void CreateSchema()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS users (
                id            INTEGER PRIMARY KEY,
                username      TEXT    NOT NULL UNIQUE,
                age           INTEGER NOT NULL,
                email         TEXT    NOT NULL,
                password_hash TEXT    NOT NULL
            );

            CREATE TABLE IF NOT EXISTS posts (
                id           INTEGER PRIMARY KEY,
                author_id    INTEGER NOT NULL,
                text         TEXT    NOT NULL,
                image_path   TEXT    NULL,
                shares_count INTEGER NOT NULL DEFAULT 0,
                created_at   TEXT    NOT NULL
            );

            CREATE TABLE IF NOT EXISTS comments (
                id         INTEGER PRIMARY KEY,
                post_id    INTEGER NOT NULL,
                author_id  INTEGER NOT NULL,
                text       TEXT    NOT NULL,
                created_at TEXT    NOT NULL
            );

            CREATE TABLE IF NOT EXISTS reactions (
                post_id INTEGER NOT NULL,
                user_id INTEGER NOT NULL,
                type    INTEGER NOT NULL,
                PRIMARY KEY (post_id, user_id)
            );
            """;
        command.ExecuteNonQuery();
    }
}
