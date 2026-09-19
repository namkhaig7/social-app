using SocialApp.Core.Models;
using SocialApp.Core.Repositories;

namespace SocialApp.Core.Services;

/// <summary>Business logic for users. The console app calls this instead of the repository directly.</summary>
public class UserService
{
    private readonly UserRepository _userRepository;
    private int _nextId = 1;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public User Register(string username, byte age, string email)
    {
        var user = new User(_nextId++, username, age, email);
        _userRepository.Add(user);
        return user;
    }

    public IReadOnlyList<User> GetAllUsers() => _userRepository.GetAll();
}
