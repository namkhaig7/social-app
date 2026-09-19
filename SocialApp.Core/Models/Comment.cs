using SocialApp.Core.Interfaces;

namespace SocialApp.Core.Models;

/// <summary>A comment left on a post. Can be liked, but not shared or commented on.</summary>
public class Comment : Content, ILikeable
{
    public int LikesCount { get; private set; }

    public Comment(int id, int authorId, string text) : base(id, authorId, text)
    {
    }

    public void Like() => LikesCount++;

    public void Unlike()
    {
        if (LikesCount > 0) LikesCount--;
    }

    public override string Preview() => $"Comment: \"{Text}\"";
}
