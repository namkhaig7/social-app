using SocialApp.Core.Models;

namespace SocialApp.Core.Interfaces;

// Hereglegch bur post buruus neg l reaction-toi.
// DB deer (post_id, user_id) hoslol ni PRIMARY KEY tul ene duremiig ugugdliin san oorroo barina.
public interface IReactionRepository
{
    void Set(int postId, int userId, ReactionType type);
    IReadOnlyDictionary<int, ReactionType> GetByPost(int postId);
    void DeleteByPost(int postId);
}
