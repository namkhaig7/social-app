using SocialApp.Core.Data;
using SocialApp.Core.Interfaces;
using SocialApp.Core.Models;

namespace SocialApp.Core.Repositories;

// Comment-uudiig SQLite "comments" hussegtend hadgalna
public class SqliteCommentRepository : ICommentRepository
{
    private readonly SocialAppDatabase _database;

    public SqliteCommentRepository(SocialAppDatabase database) => _database = database;

    public void Add(int postId, Comment comment)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText =
            "INSERT INTO comments (id, post_id, author_id, text, created_at) VALUES ($id, $postId, $authorId, $text, $createdAt)";
        command.Parameters.AddWithValue("$id", comment.Id);
        command.Parameters.AddWithValue("$postId", postId);
        command.Parameters.AddWithValue("$authorId", comment.AuthorId);
        command.Parameters.AddWithValue("$text", comment.Text);
        command.Parameters.AddWithValue("$createdAt", comment.CreatedAt.ToString("o"));
        command.ExecuteNonQuery();
    }

    public IReadOnlyList<Comment> GetByPost(int postId)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT id, author_id, text, created_at FROM comments WHERE post_id = $postId ORDER BY id";
        command.Parameters.AddWithValue("$postId", postId);

        var comments = new List<Comment>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            comments.Add(new Comment(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                DateTime.Parse(reader.GetString(3))));
        }
        return comments;
    }

    public void DeleteByPost(int postId)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM comments WHERE post_id = $postId";
        command.Parameters.AddWithValue("$postId", postId);
        command.ExecuteNonQuery();
    }

    public int NextId()
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COALESCE(MAX(id), 0) + 1 FROM comments";
        return Convert.ToInt32(command.ExecuteScalar());
    }
}
