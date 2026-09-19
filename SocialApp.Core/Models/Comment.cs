using SocialApp.Core.Interfaces;

namespace SocialApp.Core.Models;

/// Poston comment hadgalna. Comment liketai bn
public class Comment : Content, ILikeable
{
    public int LikesCount { get; private set; }

    public Comment(int id, int authorId, string text, DateTime? createdAt = null)
        : base(id, authorId, text, createdAt)
    {
    }

    public void Like() => LikesCount++;

    public void Unlike()
    {
        if (LikesCount > 0) LikesCount--;
    }

    public override string Preview() => $"Comment: \"{Text}\"";
}
