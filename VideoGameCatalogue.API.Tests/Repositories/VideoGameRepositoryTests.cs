using Microsoft.Extensions.Time.Testing;
using VideoGameCatalogue.API.Data;
using VideoGameCatalogue.API.Repositories;

namespace VideoGameCatalogue.API.Tests.Repositories
{
    public sealed class VideoGameRepositoryTests : IDisposable
    {
        private static readonly DateTimeOffset StartTime = new(2025, 6, 1, 12, 0, 0, TimeSpan.Zero);

        private readonly SqliteTestDatabase _database = new();
        private readonly FakeTimeProvider _timeProvider = new(StartTime);
        private readonly VideoGameContext _context;
        private readonly VideoGameRepository _repository;

        public VideoGameRepositoryTests()
        {
            _context = _database.CreateContext();
            _repository = new VideoGameRepository(_context, _timeProvider);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsGamesOrderedByTitle()
        {
            await _repository.CreateAsync(TestData.ValidGame("Zelda"));
            await _repository.CreateAsync(TestData.ValidGame("Anno"));

            var games = await _repository.GetAllAsync();

            Assert.Equal(["Anno", "Zelda"], games.Select(g => g.Title));
        }

        [Fact]
        public async Task GetAllAsync_WithNoGames_ReturnsEmpty()
        {
            var games = await _repository.GetAllAsync();

            Assert.Empty(games);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsGame()
        {
            var created = await _repository.CreateAsync(TestData.ValidGame("Hades"));

            var game = await _repository.GetByIdAsync(created.Id);

            Assert.NotNull(game);
            Assert.Equal("Hades", game.Title);
        }

        [Fact]
        public async Task GetByIdAsync_MissingId_ReturnsNull()
        {
            var game = await _repository.GetByIdAsync(999);

            Assert.Null(game);
        }

        [Fact]
        public async Task CreateAsync_PersistsGameAndStampsTimestamps()
        {
            var created = await _repository.CreateAsync(TestData.ValidGame("Hades"));

            Assert.True(created.Id > 0);
            Assert.Equal(StartTime.UtcDateTime, created.CreatedAt);
            Assert.Equal(StartTime.UtcDateTime, created.UpdatedAt);

            using var verifyContext = _database.CreateContext();
            var stored = await verifyContext.VideoGames.FindAsync(created.Id);
            Assert.NotNull(stored);
            Assert.Equal("Hades", stored.Title);
        }

        [Fact]
        public async Task UpdateAsync_PersistsChangesAndOnlyUpdatesUpdatedAt()
        {
            var game = await _repository.CreateAsync(TestData.ValidGame("Original"));
            _timeProvider.Advance(TimeSpan.FromHours(1));

            game.Title = "Updated";
            await _repository.UpdateAsync(game);

            using var verifyContext = _database.CreateContext();
            var stored = await verifyContext.VideoGames.FindAsync(game.Id);
            Assert.NotNull(stored);
            Assert.Equal("Updated", stored.Title);
            Assert.Equal(StartTime.UtcDateTime, stored.CreatedAt);
            Assert.Equal(StartTime.AddHours(1).UtcDateTime, stored.UpdatedAt);
        }

        [Fact]
        public async Task DeleteAsync_ExistingId_RemovesGameAndReturnsTrue()
        {
            var game = await _repository.CreateAsync(TestData.ValidGame());

            var deleted = await _repository.DeleteAsync(game.Id);

            Assert.True(deleted);
            using var verifyContext = _database.CreateContext();
            Assert.Null(await verifyContext.VideoGames.FindAsync(game.Id));
        }

        [Fact]
        public async Task DeleteAsync_MissingId_ReturnsFalse()
        {
            var deleted = await _repository.DeleteAsync(999);

            Assert.False(deleted);
        }

        public void Dispose()
        {
            _context.Dispose();
            _database.Dispose();
        }
    }
}
