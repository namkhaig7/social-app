using SocialApp.Core.Models;

namespace SocialApp.Core.Interfaces;

/// <summary>Something that can receive comments.</summary>
public interface ICommentable
{
    IReadOnlyList<Comment> Comments { get; }

    void AddComment(Comment comment);
}
