using SocialApp.Core.Models;

namespace SocialApp.Core.Tests;

[TestClass]
public class SqliteUserRepositoryTests : DatabaseTestBase
{
    private static User MakeUser(int id, string username = "mandu") =>
        new(id, username, 20, $"{username}@mail.com", "HASH");

    [TestMethod]
    public void Add_ThenGetById_ReturnsSameUser()
    {
        Users.Add(MakeUser(1));

        var loaded = Users.GetById(1);

        Assert.IsNotNull(loaded);
        Assert.AreEqual(1, loaded!.Id);
        Assert.AreEqual("mandu", loaded.Username);
        Assert.AreEqual((byte)20, loaded.Age);
        Assert.AreEqual("mandu@mail.com", loaded.Email);
        Assert.AreEqual("HASH", loaded.PasswordHash);
    }

    [TestMethod]
    public void GetById_ReturnsNull_WhenUserMissing()
    {
        Assert.IsNull(Users.GetById(999));
    }

    [TestMethod]
    public void GetAll_ReturnsEveryUser_OrderedById()
    {
        Users.Add(MakeUser(1, "mandu"));
        Users.Add(MakeUser(2, "namhai"));

        var all = Users.GetAll();

        Assert.AreEqual(2, all.Count);
        Assert.AreEqual("mandu", all[0].Username);
        Assert.AreEqual("namhai", all[1].Username);
    }

    [TestMethod]
    public void GetAll_ReturnsEmpty_WhenNoUsers()
    {
        Assert.AreEqual(0, Users.GetAll().Count);
    }

    [TestMethod]
    public void Delete_RemovesUser()
    {
        Users.Add(MakeUser(1));

        Users.Delete(1);

        Assert.IsNull(Users.GetById(1));
    }

    [TestMethod]
    public void NextId_StartsAtOne_WhenTableEmpty()
    {
        Assert.AreEqual(1, Users.NextId());
    }

    [TestMethod]
    public void NextId_ContinuesAfterHighestId()
    {
        Users.Add(MakeUser(1));
        Users.Add(MakeUser(7, "namhai"));

        Assert.AreEqual(8, Users.NextId());
    }
}
