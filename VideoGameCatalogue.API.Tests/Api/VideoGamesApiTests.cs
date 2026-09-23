using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using VideoGameCatalogue.API.Contracts;

namespace VideoGameCatalogue.API.Tests.Api
{
    /// <summary>
    /// End-to-end tests through the HTTP pipeline: routing, model binding, validation,
    /// controller, repository and database. Each test gets a fresh app and database
    /// containing the two seeded games.
    /// </summary>
    public sealed class VideoGamesApiTests : IDisposable
    {
        private const string BaseUrl = "/api/videogames";

        private readonly VideoGameCatalogueApiFactory _factory = new();
        private readonly HttpClient _client;

        public VideoGamesApiTests()
        {
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ReturnsSeededGamesOrderedByTitle()
        {
            var games = await _client.GetFromJsonAsync<List<VideoGameResponse>>(BaseUrl);

            Assert.NotNull(games);
            Assert.Equal(["Baldur's Gate 3", "The Legend of Zelda: Tears of the Kingdom"], games.Select(g => g.Title));
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsGame()
        {
            var game = await _client.GetFromJsonAsync<VideoGameResponse>($"{BaseUrl}/1");

            Assert.NotNull(game);
            Assert.Equal("The Legend of Zelda: Tears of the Kingdom", game.Title);
            Assert.Equal(new DateOnly(2023, 5, 12), game.ReleaseDate);
        }

        [Fact]
        public async Task GetById_ReturnsTimestampsMarkedAsUtc()
        {
            using var json = JsonDocument.Parse(await _client.GetStringAsync($"{BaseUrl}/1"));

            Assert.Equal("2024-01-01T00:00:00Z", json.RootElement.GetProperty("createdAt").GetString());
            Assert.Equal("2024-01-01T00:00:00Z", json.RootElement.GetProperty("updatedAt").GetString());
        }

        [Fact]
        public async Task GetById_MissingId_Returns404()
        {
            var response = await _client.GetAsync($"{BaseUrl}/999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Create_ValidRequest_Returns201WithLocationOfNewGame()
        {
            var response = await _client.PostAsJsonAsync(BaseUrl, TestData.ValidRequest());

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<VideoGameResponse>();
            Assert.NotNull(created);
            Assert.Equal("Elden Ring", created.Title);
            Assert.Equal(VideoGameCatalogueApiFactory.StartTime.UtcDateTime, created.CreatedAt);

            var fetched = await _client.GetFromJsonAsync<VideoGameResponse>(response.Headers.Location);
            Assert.Equal(created, fetched);
        }

        [Fact]
        public async Task Create_ClientSuppliedIdAndTimestamps_AreIgnored()
        {
            var body = new
            {
                id = 1,
                createdAt = new DateTime(2000, 1, 1),
                title = "Hades",
                developer = "Supergiant Games",
                publisher = "Supergiant Games",
                releaseDate = "2020-09-17",
                genre = "Action",
                price = 24.99m,
                platform = "PC",
                metacriticScore = 93
            };

            var response = await _client.PostAsJsonAsync(BaseUrl, body);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<VideoGameResponse>();
            Assert.NotNull(created);
            Assert.NotEqual(1, created.Id);
            Assert.Equal(VideoGameCatalogueApiFactory.StartTime.UtcDateTime, created.CreatedAt);
        }

        [Fact]
        public async Task Create_InvalidRequest_Returns400WithFieldErrors()
        {
            var request = TestData.ValidRequest() with { Title = new string('a', 201), Genre = "Not A Genre", MetacriticScore = 150 };

            var response = await _client.PostAsJsonAsync(BaseUrl, request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            Assert.NotNull(problem);
            Assert.Equal(["Genre", "MetacriticScore", "Title"], problem.Errors.Keys.Order(StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Create_MissingRequiredField_Returns400()
        {
            var bodyWithoutReleaseDate = new
            {
                title = "Hades",
                developer = "Supergiant Games",
                publisher = "Supergiant Games",
                genre = "Action",
                price = 24.99m,
                platform = "PC",
                metacriticScore = 93
            };

            var response = await _client.PostAsJsonAsync(BaseUrl, bodyWithoutReleaseDate);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Update_ExistingId_PersistsChangesAndUpdatesTimestamp()
        {
            _factory.TimeProvider.Advance(TimeSpan.FromDays(1));
            var request = TestData.ValidRequest() with { Title = "Updated Title" };

            var response = await _client.PutAsJsonAsync($"{BaseUrl}/1", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var fetched = await _client.GetFromJsonAsync<VideoGameResponse>($"{BaseUrl}/1");
            Assert.NotNull(fetched);
            Assert.Equal("Updated Title", fetched.Title);
            Assert.Equal(VideoGameCatalogueApiFactory.StartTime.AddDays(1).UtcDateTime, fetched.UpdatedAt);
        }

        [Fact]
        public async Task Update_MissingId_Returns404()
        {
            var response = await _client.PutAsJsonAsync($"{BaseUrl}/999", TestData.ValidRequest());

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Update_InvalidRequest_Returns400()
        {
            var response = await _client.PutAsJsonAsync($"{BaseUrl}/1", TestData.ValidRequest() with { Genre = "Not A Genre" });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ExistingId_Returns204AndRemovesGame()
        {
            var response = await _client.DeleteAsync($"{BaseUrl}/1");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            var getResponse = await _client.GetAsync($"{BaseUrl}/1");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task Delete_MissingId_Returns404()
        {
            var response = await _client.DeleteAsync($"{BaseUrl}/999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetLookups_ReturnsGenresAndPlatforms()
        {
            var lookups = await _client.GetFromJsonAsync<LookupsResponse>("/api/lookups");

            Assert.NotNull(lookups);
            Assert.Contains("Action-Adventure", lookups.Genres);
            Assert.Contains("Nintendo Switch", lookups.Platforms);
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
