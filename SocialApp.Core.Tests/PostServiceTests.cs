using SocialApp.Core.Models;
using SocialApp.Core.Services;

namespace SocialApp.Core.Tests;

// Service-uudiig jinhene SQLite repository deer ajilluulj shalgana -
// ingesneer "like/comment ni ugugdliin santai holbogdson" esehiig batalna.
[TestClass]
public class PostServiceTests : DatabaseTestBase
{
    private PostService _service = null!;

    [TestInitialize]
    public void CreateService() => _service = new PostService(Posts, Comments, Reactions);

    [TestMethod]
    public void CreatePost_SavesToDatabase()
    {
        var created = _service.CreatePost(authorId: 1, "Sain baina uu");

        var loaded = _service.GetPost(created.Id);

        Assert.IsNotNull(loaded);
        Assert.AreEqual("Sain baina uu", loaded!.Text);
    }

    [TestMethod]
    public void CreatePost_GivesEachPostItsOwnId()
    {
        var first = _service.CreatePost(1, "negdugeer");
        var second = _service.CreatePost(1, "hoyrdugaar");

        Assert.AreNotEqual(first.Id, second.Id);
        Assert.AreEqual(2, _service.GetAllPosts().Count);
    }

    [TestMethod]
    public void CreatePost_KeepsImagePath()
    {
        var created = _service.CreatePost(1, "zurag", @"C:\images\a.png");

        Assert.AreEqual(@"C:\images\a.png", _service.GetPost(created.Id)!.ImagePath);
    }

    [TestMethod]
    public void ReactPost_CountsOneReactionPerUser()
    {
        var post = _service.CreatePost(1, "post");

        _service.ReactPost(post.Id, userId: 1, ReactionType.Like);
        _service.ReactPost(post.Id, userId: 2, ReactionType.Haha);

        var loaded = _service.GetPost(post.Id)!;
        Assert.AreEqual(1, loaded.Reactions[ReactionType.Like]);
        Assert.AreEqual(1, loaded.Reactions[ReactionType.Haha]);
    }

    // Gol duram: neg hun olon udaa darval niit too ni osohgui, songolt ni l solino
    [TestMethod]
    public void ReactPost_ReplacesEarlierReactionOfSameUser()
    {
        var post = _service.CreatePost(1, "post");

        _service.ReactPost(post.Id, userId: 1, ReactionType.Sad);
        _service.ReactPost(post.Id, userId: 1, ReactionType.Angry);

        var loaded = _service.GetPost(post.Id)!;
        Assert.AreEqual(0, loaded.Reactions[ReactionType.Sad]);
        Assert.AreEqual(1, loaded.Reactions[ReactionType.Angry]);
    }

    [TestMethod]
    public void SharePost_IncreasesShareCount()
    {
        var post = _service.CreatePost(1, "post");

        _service.SharePost(post.Id);
        _service.SharePost(post.Id);

        Assert.AreEqual(2, _service.GetPost(post.Id)!.SharesCount);
    }

    [TestMethod]
    public void CommentOnPost_SavesComment()
    {
        var post = _service.CreatePost(1, "post");

        _service.CommentOnPost(post.Id, authorId: 2, "Nice post!");

        var comments = _service.GetPost(post.Id)!.Comments;
        Assert.AreEqual(1, comments.Count);
        Assert.AreEqual("Nice post!", comments[0].Text);
        Assert.AreEqual(2, comments[0].AuthorId);
    }

    [TestMethod]
    public void CommentOnPost_GivesEachCommentItsOwnId()
    {
        var post = _service.CreatePost(1, "post");

        _service.CommentOnPost(post.Id, 1, "negdugeer");
        _service.CommentOnPost(post.Id, 1, "hoyrdugaar");

        var comments = _service.GetPost(post.Id)!.Comments;
        Assert.AreEqual(2, comments.Count);
        Assert.AreNotEqual(comments[0].Id, comments[1].Id);
    }

    [TestMethod]
    public void GetPost_ReturnsNull_WhenMissing()
    {
        Assert.IsNull(_service.GetPost(404));
    }

    [TestMethod]
    public void GetAllPosts_ReturnsEmpty_WhenNoPosts()
    {
        Assert.AreEqual(0, _service.GetAllPosts().Count);
    }

    [TestMethod]
    public void DeletePost_RemovesPost()
    {
        var post = _service.CreatePost(1, "post");

        _service.DeletePost(post.Id);

        Assert.IsNull(_service.GetPost(post.Id));
    }
}
