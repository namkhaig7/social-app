using SocialApp.Core.Interfaces;
using SocialApp.Core.Models;
using SocialApp.Core.Security;

namespace SocialApp.Core.Services;

public class UserService
{
    private readonly IRepository<User> _userRepository;

    public UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public User Register(string username, byte age, string email, string password)
    {
        var user = new User(_userRepository.NextId(), username, age, email, PasswordHasher.Hash(password));
        _userRepository.Add(user);
        return user;
    }

    // Nevtreh: ner oldohgui, esvel nuuts ug buruu bol null butsaana
    public User? SignIn(string username, string password)
    {
        var user = FindByUsername(username);
        if (user is null) return null;
        return PasswordHasher.Verify(password, user.PasswordHash) ? user : null;
    }

    public User? FindByUsername(string username) =>
        _userRepository.GetAll().FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<User> GetAllUsers() => _userRepository.GetAll();

    public User? GetById(int id) => _userRepository.GetById(id);
}
