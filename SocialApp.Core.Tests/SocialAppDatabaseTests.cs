using Microsoft.Data.Sqlite;
using SocialApp.Core.Data;

namespace SocialApp.Core.Tests;

[TestClass]
public class SocialAppDatabaseTests
{
    private string _dbPath = null!;

    [TestInitialize]
    public void SetUp() =>
        _dbPath = Path.Combine(Path.GetTempPath(), $"socialapp-db-{Guid.NewGuid():N}.db");

    [TestCleanup]
    public void TearDown()
    {
        SqliteConnection.ClearAllPools();
        try
        {
            if (File.Exists(_dbPath)) File.Delete(_dbPath);
        }
        catch (IOException)
        {
        }
    }

    [TestMethod]
    public void CreateSchema_CreatesAllFourTables()
    {
        var database = new SocialAppDatabase(_dbPath);
        database.CreateSchema();

        using var connection = database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY name";

        var tables = new List<string>();
        using var reader = command.ExecuteReader();
        while (reader.Read()) tables.Add(reader.GetString(0));

        CollectionAssert.AreEquivalent(
            new[] { "users", "posts", "comments", "reactions" },
            tables);
    }

    // "CREATE TABLE IF NOT EXISTS" tul olon udaa duudsan ch aldahgui
    [TestMethod]
    public void CreateSchema_CanRunTwice()
    {
        var database = new SocialAppDatabase(_dbPath);
        database.CreateSchema();
        database.CreateSchema();

        using var connection = database.OpenConnection();
        Assert.AreEqual(System.Data.ConnectionState.Open, connection.State);
    }

    [TestMethod]
    public void OpenConnection_ReturnsOpenConnection()
    {
        var database = new SocialAppDatabase(_dbPath);
        database.CreateSchema();

        using var connection = database.OpenConnection();

        Assert.AreEqual(System.Data.ConnectionState.Open, connection.State);
    }
}
