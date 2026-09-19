using SocialApp.Core.Models;
using SocialApp.Core.Security;

namespace SocialApp.Core.Tests;

// Model bolon nuuts ugiin hash-iin engiin shalgaltuud (DB shaardlagagui)
[TestClass]
public class ModelTests
{
    [TestMethod]
    public void PasswordHasher_SamePasswordGivesSameHash()
    {
        Assert.AreEqual(PasswordHasher.Hash("1234"), PasswordHasher.Hash("1234"));
    }

    [TestMethod]
    public void PasswordHasher_DifferentPasswordsGiveDifferentHashes()
    {
        Assert.AreNotEqual(PasswordHasher.Hash("1234"), PasswordHasher.Hash("4321"));
    }

    [TestMethod]
    public void PasswordHasher_VerifyAcceptsCorrectAndRejectsWrong()
    {
        var hash = PasswordHasher.Hash("1234");

        Assert.IsTrue(PasswordHasher.Verify("1234", hash));
        Assert.IsFalse(PasswordHasher.Verify("buruu", hash));
    }

    [TestMethod]
    public void User_GetProfileInfo_ShowsUsernameAgeEmail()
    {
        var user = new User(1, "mandu", 20, "mandu@mail.com", "HASH");

        Assert.AreEqual("mandu (age 20) - mandu@mail.com", user.GetProfileInfo());
    }

    [TestMethod]
    public void Post_Preview_ShowsText()
    {
        Assert.AreEqual("Post: \"hi\"", new Post(1, 1, "hi").Preview());
    }

    [TestMethod]
    public void Comment_Preview_ShowsText()
    {
        Assert.AreEqual("Comment: \"hi\"", new Comment(1, 1, "hi").Preview());
    }

    [TestMethod]
    public void Comment_LikeAndUnlike_NeverGoesBelowZero()
    {
        var comment = new Comment(1, 1, "hi");

        comment.Unlike();
        Assert.AreEqual(0, comment.LikesCount);

        comment.Like();
        comment.Like();
        Assert.AreEqual(2, comment.LikesCount);

        comment.Unlike();
        Assert.AreEqual(1, comment.LikesCount);
    }

    [TestMethod]
    public void Post_Share_IncreasesCount()
    {
        var post = new Post(1, 1, "hi");

        post.Share();

        Assert.AreEqual(1, post.SharesCount);
    }

    [TestMethod]
    public void Post_NewPostHasZeroOfEveryReaction()
    {
        var post = new Post(1, 1, "hi");

        foreach (var type in Enum.GetValues<ReactionType>())
            Assert.AreEqual(0, post.Reactions[type]);
    }
}
