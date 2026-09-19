using SocialApp.Core.Models;

namespace SocialApp.Core.Tests;

[TestClass]
public class SqlitePostRepositoryTests : DatabaseTestBase
{
    [TestMethod]
    public void Add_ThenGetById_ReturnsSamePost()
    {
        Posts.Add(new Post(1, authorId: 2, "Sain baina uu"));

        var loaded = Posts.GetById(1);

        Assert.IsNotNull(loaded);
        Assert.AreEqual(1, loaded!.Id);
        Assert.AreEqual(2, loaded.AuthorId);
        Assert.AreEqual("Sain baina uu", loaded.Text);
        Assert.IsNull(loaded.ImagePath);
        Assert.AreEqual(0, loaded.SharesCount);
    }

    [TestMethod]
    public void Add_KeepsImagePathAndCreatedAt()
    {
        var created = new DateTime(2026, 4, 1, 12, 0, 0);
        Posts.Add(new Post(1, 2, "zurag", @"C:\images\a.png", sharesCount: 3, createdAt: created));

        var loaded = Posts.GetById(1);

        Assert.AreEqual(@"C:\images\a.png", loaded!.ImagePath);
        Assert.AreEqual(3, loaded.SharesCount);
        Assert.AreEqual(created, loaded.CreatedAt);
    }

    [TestMethod]
    public void GetById_ReturnsNull_WhenPostMissing()
    {
        Assert.IsNull(Posts.GetById(404));
    }

    // Post ni oorin comment, reaction-uudaa DB-ees duurgej avdag esehiig shalgana
    [TestMethod]
    public void GetById_LoadsCommentsAndReactions()
    {
        Posts.Add(new Post(1, 2, "post"));
        Comments.Add(1, new Comment(1, authorId: 3, "Nice post!"));
        Reactions.Set(1, userId: 3, ReactionType.Haha);
        Reactions.Set(1, userId: 4, ReactionType.Haha);

        var loaded = Posts.GetById(1)!;

        Assert.AreEqual(1, loaded.Comments.Count);
        Assert.AreEqual("Nice post!", loaded.Comments[0].Text);
        Assert.AreEqual(2, loaded.Reactions[ReactionType.Haha]);
        Assert.AreEqual(0, loaded.Reactions[ReactionType.Like]);
    }

    [TestMethod]
    public void GetAll_ReturnsEveryPost_OrderedById()
    {
        Posts.Add(new Post(1, 1, "negdugeer"));
        Posts.Add(new Post(2, 1, "hoyrdugaar"));

        var all = Posts.GetAll();

        Assert.AreEqual(2, all.Count);
        Assert.AreEqual("negdugeer", all[0].Text);
        Assert.AreEqual("hoyrdugaar", all[1].Text);
    }

    [TestMethod]
    public void GetAll_LoadsCommentsForEachPost()
    {
        Posts.Add(new Post(1, 1, "post"));
        Comments.Add(1, new Comment(1, 1, "comment"));

        var all = Posts.GetAll();

        Assert.AreEqual(1, all[0].Comments.Count);
    }

    [TestMethod]
    public void GetAll_ReturnsEmpty_WhenNoPosts()
    {
        Assert.AreEqual(0, Posts.GetAll().Count);
    }

    [TestMethod]
    public void Delete_AlsoRemovesCommentsAndReactions()
    {
        Posts.Add(new Post(1, 1, "post"));
        Comments.Add(1, new Comment(1, 1, "comment"));
        Reactions.Set(1, 1, ReactionType.Like);

        Posts.Delete(1);

        Assert.IsNull(Posts.GetById(1));
        Assert.AreEqual(0, Comments.GetByPost(1).Count);
        Assert.AreEqual(0, Reactions.GetByPost(1).Count);
    }

    [TestMethod]
    public void IncrementShares_AddsOneEachTime()
    {
        Posts.Add(new Post(1, 1, "post"));

        Posts.IncrementShares(1);
        Posts.IncrementShares(1);

        Assert.AreEqual(2, Posts.GetById(1)!.SharesCount);
    }

    [TestMethod]
    public void NextId_StartsAtOne_ThenFollowsHighestId()
    {
        Assert.AreEqual(1, Posts.NextId());

        Posts.Add(new Post(3, 1, "post"));

        Assert.AreEqual(4, Posts.NextId());
    }
}
