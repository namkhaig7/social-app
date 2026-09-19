using SocialApp.Core.Models;

namespace SocialApp.Core.Tests;

[TestClass]
public class SqliteCommentRepositoryTests : DatabaseTestBase
{
    [TestMethod]
    public void Add_ThenGetByPost_ReturnsComment()
    {
        Comments.Add(postId: 5, new Comment(1, authorId: 2, "Nice post!"));

        var loaded = Comments.GetByPost(5);

        Assert.AreEqual(1, loaded.Count);
        Assert.AreEqual(1, loaded[0].Id);
        Assert.AreEqual(2, loaded[0].AuthorId);
        Assert.AreEqual("Nice post!", loaded[0].Text);
    }

    [TestMethod]
    public void GetByPost_KeepsOriginalCreatedAt()
    {
        var created = new DateTime(2026, 3, 14, 9, 30, 0);
        Comments.Add(5, new Comment(1, 2, "Hi", created));

        var loaded = Comments.GetByPost(5);

        Assert.AreEqual(created, loaded[0].CreatedAt);
    }

    [TestMethod]
    public void GetByPost_ReturnsOnlyThatPostsComments()
    {
        Comments.Add(1, new Comment(1, 1, "on post 1"));
        Comments.Add(2, new Comment(2, 1, "on post 2"));

        var loaded = Comments.GetByPost(1);

        Assert.AreEqual(1, loaded.Count);
        Assert.AreEqual("on post 1", loaded[0].Text);
    }

    [TestMethod]
    public void GetByPost_ReturnsEmpty_WhenNoComments()
    {
        Assert.AreEqual(0, Comments.GetByPost(42).Count);
    }

    [TestMethod]
    public void DeleteByPost_RemovesOnlyThatPostsComments()
    {
        Comments.Add(1, new Comment(1, 1, "keep me out"));
        Comments.Add(2, new Comment(2, 1, "keep me"));

        Comments.DeleteByPost(1);

        Assert.AreEqual(0, Comments.GetByPost(1).Count);
        Assert.AreEqual(1, Comments.GetByPost(2).Count);
    }

    [TestMethod]
    public void NextId_StartsAtOne_ThenFollowsHighestId()
    {
        Assert.AreEqual(1, Comments.NextId());

        Comments.Add(1, new Comment(4, 1, "hi"));

        Assert.AreEqual(5, Comments.NextId());
    }
}
