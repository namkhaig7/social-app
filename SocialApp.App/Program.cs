using SocialApp.Core.Data;
using SocialApp.Core.Models;
using SocialApp.Core.Repositories;
using SocialApp.Core.Services;

// Demo tul ajillah bur shineer ehluulne
const string DbFile = "socialapp-demo.db";
if (File.Exists(DbFile)) File.Delete(DbFile);

var database = new SocialAppDatabase(DbFile);
database.CreateSchema();

var commentRepository = new SqliteCommentRepository(database);
var reactionRepository = new SqliteReactionRepository(database);

var userService = new UserService(new SqliteUserRepository(database));
var postService = new PostService(
    new SqlitePostRepository(database, commentRepository, reactionRepository),
    commentRepository,
    reactionRepository);

var mandu = userService.Register("mandu", 20, "mandu@mail.com", "1234");
var namhai = userService.Register("namhai", 21, "namhai@mail.com", "1234");

Console.WriteLine("--- Users ---");
foreach (var user in userService.GetAllUsers())
    Console.WriteLine(user.GetProfileInfo());

// Mandu posts smth
var post = postService.CreatePost(mandu.Id, "Sain baitsgaana uu, Yu bn!");

postService.ReactPost(post.Id, mandu.Id, ReactionType.Like);
postService.ReactPost(post.Id, namhai.Id, ReactionType.Haha);

postService.ReactPost(post.Id, namhai.Id, ReactionType.Angry);
postService.CommentOnPost(post.Id, namhai.Id, "Nice post!");
postService.SharePost(post.Id);

Console.WriteLine("\n--- Posts ---");
foreach (var p in postService.GetAllPosts())
{
    Console.WriteLine(p.Preview());
    var reactions = string.Join(", ", p.Reactions.Select(r => $"{r.Key}:{r.Value}"));
    Console.WriteLine($"  Reactions: {reactions}, Shares: {p.SharesCount}");
    foreach (var comment in p.Comments)
        Console.WriteLine($"  - {comment.Preview()}");
}
