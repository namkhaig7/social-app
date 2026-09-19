using SocialApp.Core.Models;

namespace SocialApp.Core.Tests;

[TestClass]
public class SqliteReactionRepositoryTests : DatabaseTestBase
{
    [TestMethod]
    public void Set_ThenGetByPost_ReturnsReaction()
    {
        Reactions.Set(postId: 1, userId: 7, ReactionType.Haha);

        var loaded = Reactions.GetByPost(1);

        Assert.HasCount(1, loaded);
        Assert.AreEqual(ReactionType.Haha, loaded[7]);
    }

    // Ene bol gol duram: neg hereglegch dahin darval nemegdehgui, solino
    [TestMethod]
    public void Set_ReplacesReaction_WhenSameUserReactsAgain()
    {
        Reactions.Set(1, userId: 7, ReactionType.Sad);
        Reactions.Set(1, userId: 7, ReactionType.Angry);

        var loaded = Reactions.GetByPost(1);

        Assert.HasCount(1, loaded, "neg hereglegch neg l reactiontai baih ystoi");
        Assert.AreEqual(ReactionType.Angry, loaded[7]);
    }

    [TestMethod]
    public void Set_KeepsReactionsOfDifferentUsersSeparate()
    {
        Reactions.Set(1, 7, ReactionType.Like);
        Reactions.Set(1, 8, ReactionType.Angry);

        var loaded = Reactions.GetByPost(1);

        Assert.HasCount(2, loaded);
        Assert.AreEqual(ReactionType.Like, loaded[7]);
        Assert.AreEqual(ReactionType.Angry, loaded[8]);
    }

    [TestMethod]
    public void GetByPost_ReturnsOnlyThatPostsReactions()
    {
        Reactions.Set(1, 7, ReactionType.Like);
        Reactions.Set(2, 7, ReactionType.Sad);

        Assert.AreEqual(ReactionType.Like, Reactions.GetByPost(1)[7]);
        Assert.AreEqual(ReactionType.Sad, Reactions.GetByPost(2)[7]);
    }

    [TestMethod]
    public void GetByPost_ReturnsEmpty_WhenNoReactions()
    {
        Assert.IsEmpty(Reactions.GetByPost(99));
    }

    [TestMethod]
    public void DeleteByPost_RemovesOnlyThatPostsReactions()
    {
        Reactions.Set(1, 7, ReactionType.Like);
        Reactions.Set(2, 7, ReactionType.Like);

        Reactions.DeleteByPost(1);

        Assert.IsEmpty(Reactions.GetByPost(1));
        Assert.HasCount(1, Reactions.GetByPost(2));
    }
}
