using SocialApp.Core.Interfaces;
using SocialApp.Core.Models;

namespace SocialApp.Core.Services;

// Post logic: create, reaction, comment, share.
// Interface-eer ajilladag tul ugugdel haana hadgalagdahiig  medeh shaardlagagui.
public class PostService
{
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IReactionRepository _reactionRepository;

    public PostService(
        IPostRepository postRepository,
        ICommentRepository commentRepository,
        IReactionRepository reactionRepository)
    {
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _reactionRepository = reactionRepository;
    }

    public Post CreatePost(int authorId, string text, string? imagePath = null)
    {
        var post = new Post(_postRepository.NextId(), authorId, text, imagePath);
        _postRepository.Add(post);
        return post;
    }

    // Neg hereglegch neg l reaction-toi - dahin darval hunii songolt ni solino
    public void ReactPost(int postId, int userId, ReactionType type) =>
        _reactionRepository.Set(postId, userId, type);

    public void SharePost(int postId) => _postRepository.IncrementShares(postId);

    public void CommentOnPost(int postId, int authorId, string text)
    {
        var comment = new Comment(_commentRepository.NextId(), authorId, text);
        _commentRepository.Add(postId, comment);
    }

    public Post? GetPost(int postId) => _postRepository.GetById(postId);

    public IReadOnlyList<Post> GetAllPosts() => _postRepository.GetAll();

    public void DeletePost(int postId) => _postRepository.Delete(postId);
}
