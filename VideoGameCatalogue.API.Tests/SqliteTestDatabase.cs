using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.API.Data;

namespace VideoGameCatalogue.API.Tests
{
    /// <summary>
    /// An in-memory SQLite database that lives as long as this object. Unlike the EF InMemory
    /// provider it is a real relational database, so NOT NULL constraints, keys and SQL
    /// translation (e.g. ExecuteDelete) behave as they do in production.
    /// </summary>
    public sealed class SqliteTestDatabase : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<VideoGameContext> _options;

        public SqliteTestDatabase(bool includeSeedData = false)
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<VideoGameContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = CreateContext();
            context.Database.EnsureCreated();

            if (!includeSeedData)
                context.VideoGames.ExecuteDelete();
        }

        public VideoGameContext CreateContext() => new(_options);

        public void Dispose() => _connection.Dispose();
    }
}
