using SocialApp.Core.Repositories;
using SocialApp.Core.Services;

// Wire up repositories and services (this is the only place that knows about both).
var userService = new UserService(new UserRepository());
var postService = new PostService(new PostRepository());

var mandu = userService.Register("mandu", 20, "mandu@mail.com");
var namhai = userService.Register("namhai", 21, "namhai@mail.com");

Console.WriteLine("--- Users ---");
foreach (var user in userService.GetAllUsers())
    Console.WriteLine(user.GetProfileInfo());

// Mandu posts smth
var post = postService.CreatePost(mandu.Id, "Sain baitsgaana uu, Yu bn!");

// tuhain posttoi haritsaj bgaa ni
postService.LikePost(post.Id);
postService.LikePost(post.Id);
postService.CommentOnPost(post.Id, bob.Id, "Nice post!");
postService.SharePost(post.Id);

Console.WriteLine("\n--- Posts ---");
foreach (var p in postService.GetAllPosts())
{
    Console.WriteLine(p.Preview());
    Console.WriteLine($"  Likes: {p.LikesCount}, Shares: {p.SharesCount}");
    foreach (var comment in p.Comments)
        Console.WriteLine($"  - {comment.Preview()}");
}
