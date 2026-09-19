using SocialApp.Core.Data;
using SocialApp.Core.Models;
using SocialApp.Core.Repositories;
using SocialApp.Core.Services;

namespace SocialApp.Desktop;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // SQLite file programm deerh haalgand uusne - ugugdel hadgalagdaj uldene
        var dbPath = Path.Combine(Application.StartupPath, "socialapp.db");
        var database = new SocialAppDatabase(dbPath);
        database.CreateSchema();

        var commentRepository = new SqliteCommentRepository(database);
        var reactionRepository = new SqliteReactionRepository(database);

        var userService = new UserService(new SqliteUserRepository(database));
        var postService = new PostService(
            new SqlitePostRepository(database, commentRepository, reactionRepository),
            commentRepository,
            reactionRepository);

        if (userService.GetAllUsers().Count == 0)
        {
            var mandu = userService.Register("mandu", 20, "mandu@mail.com", "1234");
            userService.Register("namhai", 21, "namhai@mail.com", "1234");
            postService.CreatePost(mandu.Id, "Sain baina uu, ene bol minii ehnii post!");
        }

        // Login -> feed -> logout bolvol dahiad login
        while (true)
        {
            User? signedInUser;
            using (var login = new LoginForm(userService))
            {
                if (login.ShowDialog() != DialogResult.OK) return;
                signedInUser = login.SignedInUser;
            }

            var main = new Form1(userService, postService, signedInUser!);
            Application.Run(main);

            // Logout bish, tsonhoo haasan bol programm duusna
            if (!main.LoggedOut) return;
        }
    }
}
