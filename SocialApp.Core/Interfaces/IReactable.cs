using SocialApp.Core.Models;

namespace SocialApp.Core.Interfaces;

// Poston deer reaction (like/haha/sad/angry) darj boloh
public interface IReactable
{
    IReadOnlyDictionary<ReactionType, int> Reactions { get; }

    // userId - neg hereglegch neg l reactiontai, dahin darval solig dana
    void React(int userId, ReactionType type);
}
