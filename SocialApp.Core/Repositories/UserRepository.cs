using SocialApp.Core.Interfaces;
using SocialApp.Core.Models;

namespace SocialApp.Core.Repositories;

/// <summary>Keeps users in memory. Swap this out later for a real database without touching Services.</summary>
public class UserRepository : IRepository<User>
{
    private readonly List<User> _users = [];

    public void Add(User item) => _users.Add(item);

    public User? GetById(int id) => _users.FirstOrDefault(u => u.Id == id);

    public IReadOnlyList<User> GetAll() => _users;

    public void Delete(int id) => _users.RemoveAll(u => u.Id == id);
}
