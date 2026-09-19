using SocialApp.Core.Models;

namespace SocialApp.Core.Interfaces;

// Post-d zowhon hamaarah nemelt uildel
public interface IPostRepository : IRepository<Post>
{
    void IncrementShares(int postId);
}
