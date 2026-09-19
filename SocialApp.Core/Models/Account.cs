namespace SocialApp.Core.Models;

/// <summary>
/// Base class for any account on the platform.
/// Abstract because a bare "Account" should never be created directly.
/// </summary>
public abstract class Account
{
    public int Id { get; }
    public string Username { get; }

    // Age never changes after the account is created, so it is readonly.
    // byte is enough since a person's age never needs to be bigger than 255.
    public byte Age { get; }

    protected Account(int id, string username, byte age)
    {
        Id = id;
        Username = username;
        Age = age;
    }

    /// <summary>Each concrete account type describes itself differently.</summary>
    public abstract string GetProfileInfo();
}
