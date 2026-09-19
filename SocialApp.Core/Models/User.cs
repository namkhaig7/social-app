namespace SocialApp.Core.Models;

public class User : Account
{
    public string Email { get; }

    public User(int id, string username, byte age, string email, string passwordHash)
        : base(id, username, age, passwordHash)
    {
        Email = email;
    }

    public override string GetProfileInfo() =>
        $"{Username} (age {Age}) - {Email}";
}
