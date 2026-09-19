namespace SocialApp.Core.Models;

/// <summary>A regular user account. Inherits the shared Account fields.</summary>
public class User : Account
{
    public string Email { get; }

    public User(int id, string username, byte age, string email)
        : base(id, username, age)
    {
        Email = email;
    }

    public override string GetProfileInfo() =>
        $"{Username} (age {Age}) - {Email}";
}
