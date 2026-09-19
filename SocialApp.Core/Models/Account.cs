namespace SocialApp.Core.Models;

//Account id username, age-tei
public abstract class Account
{
    public int Id { get; }
    public string Username { get; }

    public byte Age { get; }

    // Nuuts ugiin hash (iltsee nuuts ug BISH) - nevtreh uyd shalgahad ashiglana
    public string PasswordHash { get; }

    protected Account(int id, string username, byte age, string passwordHash)
    {
        Id = id;
        Username = username;
        Age = age;
        PasswordHash = passwordHash;
    }

    public abstract string GetProfileInfo();
}
