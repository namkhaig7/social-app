using Microsoft.Data.Sqlite;
using SocialApp.Core.Data;
using SocialApp.Core.Interfaces;
using SocialApp.Core.Models;

namespace SocialApp.Core.Repositories;

// post-iig unshihdaa comment/reaction repository-uudaas ni avna.
public class SqlitePostRepository : IPostRepository
{
    private readonly SocialAppDatabase _database;
    private readonly ICommentRepository _comments;
    private readonly IReactionRepository _reactions;

    public SqlitePostRepository(
        SocialAppDatabase database,
        ICommentRepository comments,
        IReactionRepository reactions)
    {
        _database = database;
        _comments = comments;
        _reactions = reactions;
    }

    public void Add(Post item)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO posts (id, author_id, text, image_path, shares_count, created_at)
            VALUES ($id, $authorId, $text, $imagePath, $shares, $createdAt)
            """;
        command.Parameters.AddWithValue("$id", item.Id);
        command.Parameters.AddWithValue("$authorId", item.AuthorId);
        command.Parameters.AddWithValue("$text", item.Text);
        command.Parameters.AddWithValue("$imagePath", (object?)item.ImagePath ?? DBNull.Value);
        command.Parameters.AddWithValue("$shares", item.SharesCount);
        command.Parameters.AddWithValue("$createdAt", item.CreatedAt.ToString("o"));
        command.ExecuteNonQuery();
    }

    public Post? GetById(int id)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT id, author_id, text, image_path, shares_count, created_at FROM posts WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? Hydrate(reader) : null;
    }

    public IReadOnlyList<Post> GetAll()
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT id, author_id, text, image_path, shares_count, created_at FROM posts ORDER BY id";

        var posts = new List<Post>();
        using var reader = command.ExecuteReader();
        while (reader.Read()) posts.Add(Hydrate(reader));
        return posts;
    }

    // Post ustgahad tuunii comment, reaction-uud ch hamt ustana
    public void Delete(int id)
    {
        _comments.DeleteByPost(id);
        _reactions.DeleteByPost(id);

        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM posts WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    public void IncrementShares(int postId)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE posts SET shares_count = shares_count + 1 WHERE id = $id";
        command.Parameters.AddWithValue("$id", postId);
        command.ExecuteNonQuery();
    }

    public int NextId()
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COALESCE(MAX(id), 0) + 1 FROM posts";
        return Convert.ToInt32(command.ExecuteScalar());
    }

    // Muriig Post bolgood, comment/reaction-uudiig ni doroos ni duurgene
    private Post Hydrate(SqliteDataReader reader)
    {
        var post = new Post(
            reader.GetInt32(0),
            reader.GetInt32(1),
            reader.GetString(2),
            reader.IsDBNull(3) ? null : reader.GetString(3),
            reader.GetInt32(4),
            DateTime.Parse(reader.GetString(5)));

        foreach (var comment in _comments.GetByPost(post.Id))
            post.AddComment(comment);

        foreach (var (userId, type) in _reactions.GetByPost(post.Id))
            post.React(userId, type);

        return post;
    }
}
