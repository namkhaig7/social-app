using SocialApp.Core.Data;
using SocialApp.Core.Interfaces;
using SocialApp.Core.Models;

namespace SocialApp.Core.Repositories;

// Hereglegchiig SQLite "users" hussegtend hadgalna
public class SqliteUserRepository : IRepository<User>
{
    private readonly SocialAppDatabase _database;

    public SqliteUserRepository(SocialAppDatabase database) => _database = database;

    public void Add(User item)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText =
            "INSERT INTO users (id, username, age, email, password_hash) VALUES ($id, $username, $age, $email, $hash)";
        command.Parameters.AddWithValue("$id", item.Id);
        command.Parameters.AddWithValue("$username", item.Username);
        command.Parameters.AddWithValue("$age", item.Age);
        command.Parameters.AddWithValue("$email", item.Email);
        command.Parameters.AddWithValue("$hash", item.PasswordHash);
        command.ExecuteNonQuery();
    }

    public User? GetById(int id)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, username, age, email, password_hash FROM users WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? ReadUser(reader) : null;
    }

    public IReadOnlyList<User> GetAll()
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, username, age, email, password_hash FROM users ORDER BY id";

        var users = new List<User>();
        using var reader = command.ExecuteReader();
        while (reader.Read()) users.Add(ReadUser(reader));
        return users;
    }

    public void Delete(int id)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM users WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    public int NextId()
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COALESCE(MAX(id), 0) + 1 FROM users";
        return Convert.ToInt32(command.ExecuteScalar());
    }

    private static User ReadUser(Microsoft.Data.Sqlite.SqliteDataReader reader) =>
        new(
            reader.GetInt32(0),
            reader.GetString(1),
            (byte)reader.GetInt32(2),
            reader.GetString(3),
            reader.GetString(4));
}
