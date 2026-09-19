using SocialApp.Core.Interfaces;
using SocialApp.Core.Models;

namespace SocialApp.Core.Repositories;

/// <summary>Keeps posts in memory.</summary>
public class PostRepository : IRepository<Post>
{
    private readonly List<Post> _posts = [];

    public void Add(Post item) => _posts.Add(item);

    public Post? GetById(int id) => _posts.FirstOrDefault(p => p.Id == id);

    public IReadOnlyList<Post> GetAll() => _posts;

    public void Delete(int id) => _posts.RemoveAll(p => p.Id == id);
}
