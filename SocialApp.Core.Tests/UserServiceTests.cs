using SocialApp.Core.Security;
using SocialApp.Core.Services;

namespace SocialApp.Core.Tests;

[TestClass]
public class UserServiceTests : DatabaseTestBase
{
    private UserService _service = null!;

    [TestInitialize]
    public void CreateService() => _service = new UserService(Users);

    [TestMethod]
    public void Register_SavesUserToDatabase()
    {
        var user = _service.Register("mandu", 20, "mandu@mail.com", "1234");

        var loaded = _service.GetById(user.Id);

        Assert.IsNotNull(loaded);
        Assert.AreEqual("mandu", loaded!.Username);
        Assert.AreEqual((byte)20, loaded.Age);
    }

    // Nuuts ug hezee ch iltseeree hadgalagdahgui
    [TestMethod]
    public void Register_StoresHashNotPlainPassword()
    {
        var user = _service.Register("mandu", 20, "mandu@mail.com", "1234");

        Assert.AreNotEqual("1234", user.PasswordHash);
        Assert.AreEqual(PasswordHasher.Hash("1234"), user.PasswordHash);
    }

    [TestMethod]
    public void Register_GivesEachUserItsOwnId()
    {
        var first = _service.Register("mandu", 20, "a@mail.com", "1234");
        var second = _service.Register("namhai", 21, "b@mail.com", "1234");

        Assert.AreNotEqual(first.Id, second.Id);
        Assert.HasCount(2, _service.GetAllUsers());
    }

    [TestMethod]
    public void SignIn_ReturnsUser_WhenPasswordCorrect()
    {
        _service.Register("mandu", 20, "mandu@mail.com", "1234");

        var user = _service.SignIn("mandu", "1234");

        Assert.IsNotNull(user);
        Assert.AreEqual("mandu", user!.Username);
    }

    [TestMethod]
    public void SignIn_IgnoresUsernameCasing()
    {
        _service.Register("mandu", 20, "mandu@mail.com", "1234");

        Assert.IsNotNull(_service.SignIn("MANDU", "1234"));
    }

    [TestMethod]
    public void SignIn_ReturnsNull_WhenPasswordWrong()
    {
        _service.Register("mandu", 20, "mandu@mail.com", "1234");

        Assert.IsNull(_service.SignIn("mandu", "buruu"));
    }

    [TestMethod]
    public void SignIn_ReturnsNull_WhenUserUnknown()
    {
        Assert.IsNull(_service.SignIn("baihgui", "1234"));
    }

    [TestMethod]
    public void FindByUsername_ReturnsNull_WhenUserUnknown()
    {
        Assert.IsNull(_service.FindByUsername("baihgui"));
    }

    [TestMethod]
    public void GetById_ReturnsNull_WhenUserUnknown()
    {
        Assert.IsNull(_service.GetById(404));
    }

    [TestMethod]
    public void GetAllUsers_ReturnsEmpty_WhenNoUsers()
    {
        Assert.AreEqual(0, _service.GetAllUsers().Count);
    }
}
