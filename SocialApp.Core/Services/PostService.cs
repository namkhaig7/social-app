using SocialApp.Core.Models;
using SocialApp.Core.Repositories;

namespace SocialApp.Core.Services;

/// <summary>Business logic for posts: create, like, comment, share.</summary>
public class PostService
{
    private readonly PostRepository _postRepository;
    private int _nextPostId = 1;
    private int _nextCommentId = 1;

    public PostService(PostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public Post CreatePost(int authorId, string text)
    {
        var post = new Post(_nextPostId++, authorId, text);
        _postRepository.Add(post);
        return post;
    }

    public void LikePost(int postId) => _postRepository.GetById(postId)?.Like();

    public void SharePost(int postId) => _postRepository.GetById(postId)?.Share();

    public void CommentOnPost(int postId, int authorId, string text)
    {
        var post = _postRepository.GetById(postId);
        if (post is null) return;

        var comment = new Comment(_nextCommentId++, authorId, text);
        post.AddComment(comment);
    }

    public IReadOnlyList<Post> GetAllPosts() => _postRepository.GetAll();
}
