namespace SocialApp.Core.Models;

/// <summary>
/// Base class for anything a user publishes (post, comment, ...).
/// Abstract because "Content" on its own is not a real thing you post.
/// </summary>
public abstract class Content
{
    public int Id { get; }
    public int AuthorId { get; }
    public string Text { get; }
    public DateTime CreatedAt { get; }

    protected Content(int id, int authorId, string text)
    {
        Id = id;
        AuthorId = authorId;
        Text = text;
        CreatedAt = DateTime.Now;
    }

    /// <summary>Short one-line preview, different for each content type.</summary>
    public abstract string Preview();
}
