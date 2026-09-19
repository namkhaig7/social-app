using SocialApp.Core.Interfaces;

namespace SocialApp.Core.Models;

/// <summary>
/// A post. Inherits Content (abstract class) and implements three
/// interfaces to show all the actions a post supports: like, comment, share.
/// </summary>
public class Post : Content, ILikeable, ICommentable, IShareable
{
    private readonly List<Comment> _comments = [];

    public int LikesCount { get; private set; }
    public int SharesCount { get; private set; }
    public IReadOnlyList<Comment> Comments => _comments;

    public Post(int id, int authorId, string text) : base(id, authorId, text)
    {
    }

    public void Like() => LikesCount++;

    public void Unlike()
    {
        if (LikesCount > 0) LikesCount--;
    }

    public void Share() => SharesCount++;

    public void AddComment(Comment comment) => _comments.Add(comment);

    public override string Preview() => $"Post: \"{Text}\"";
}
