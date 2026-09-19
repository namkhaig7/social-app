using SocialApp.Core.Data;
using SocialApp.Core.Interfaces;
using SocialApp.Core.Models;

namespace SocialApp.Core.Repositories;

// Reaction-uudiig SQLite "reactions" hussegtend hadgalna
public class SqliteReactionRepository : IReactionRepository
{
    private readonly SocialAppDatabase _database;

    public SqliteReactionRepository(SocialAppDatabase database) => _database = database;

    // Darsan reaction aa uurchluh
    public void Set(int postId, int userId, ReactionType type)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO reactions (post_id, user_id, type)
            VALUES ($postId, $userId, $type)
            ON CONFLICT(post_id, user_id) DO UPDATE SET type = $type
            """;
        command.Parameters.AddWithValue("$postId", postId);
        command.Parameters.AddWithValue("$userId", userId);
        command.Parameters.AddWithValue("$type", (int)type);
        command.ExecuteNonQuery();
    }

    public IReadOnlyDictionary<int, ReactionType> GetByPost(int postId)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT user_id, type FROM reactions WHERE post_id = $postId";
        command.Parameters.AddWithValue("$postId", postId);

        var reactions = new Dictionary<int, ReactionType>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
            reactions[reader.GetInt32(0)] = (ReactionType)reader.GetInt32(1);
        return reactions;
    }

    public void DeleteByPost(int postId)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM reactions WHERE post_id = $postId";
        command.Parameters.AddWithValue("$postId", postId);
        command.ExecuteNonQuery();
    }
}
