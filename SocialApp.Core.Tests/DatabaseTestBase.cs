using Microsoft.Data.Sqlite;
using SocialApp.Core.Data;
using SocialApp.Core.Repositories;

namespace SocialApp.Core.Tests;

// Test bur oorin tusdaa hooson DB file deer ajillana - ingesneer test-uud
// bie bieded nuluulehgui.
public abstract class DatabaseTestBase
{
    private string _dbPath = null!;

    protected SocialAppDatabase Database { get; private set; } = null!;
    protected SqliteUserRepository Users { get; private set; } = null!;
    protected SqliteCommentRepository Comments { get; private set; } = null!;
    protected SqliteReactionRepository Reactions { get; private set; } = null!;
    protected SqlitePostRepository Posts { get; private set; } = null!;

    [TestInitialize]
    public void SetUp()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"socialapp-test-{Guid.NewGuid():N}.db");
        Database = new SocialAppDatabase(_dbPath);
        Database.CreateSchema();

        Users = new SqliteUserRepository(Database);
        Comments = new SqliteCommentRepository(Database);
        Reactions = new SqliteReactionRepository(Database);
        Posts = new SqlitePostRepository(Database, Comments, Reactions);
    }

    [TestCleanup]
    public void TearDown()
    {
        // Holbolt pool-d uldvel file ustahgui tul ehleed tsewerlene.
        // File ustgaj chadahgui bol test-iig unagaah shaardlagagui - temp dotor l uldene.
        SqliteConnection.ClearAllPools();
        try
        {
            if (File.Exists(_dbPath)) File.Delete(_dbPath);
        }
        catch (IOException)
        {
        }
    }
}
