using SocialApp.Core.Models;

namespace SocialApp.Core.Interfaces;

// Comment-uud ali post-d haryalagdahiig hadgalna
public interface ICommentRepository
{
    void Add(int postId, Comment comment);
    IReadOnlyList<Comment> GetByPost(int postId);
    void DeleteByPost(int postId);
    int NextId();
}
