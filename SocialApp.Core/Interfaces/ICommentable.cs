using SocialApp.Core.Models;

namespace SocialApp.Core.Interfaces;

//Comment bichdeg interface
public interface ICommentable
{
    IReadOnlyList<Comment> Comments { get; }

    void AddComment(Comment comment);
}
