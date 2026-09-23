using VideoGameCatalogue.API.Contracts;
using VideoGameCatalogue.API.Models;

namespace VideoGameCatalogue.API.Tests
{
    /// <summary>
    /// Builders for valid test objects. Tests override only the fields they care about,
    /// e.g. <c>TestData.ValidRequest() with { Title = "" }</c>.
    /// </summary>
    public static class TestData
    {
        public static VideoGameRequest ValidRequest() => new()
        {
            Title = "Elden Ring",
            Developer = "FromSoftware",
            Publisher = "Bandai Namco",
            ReleaseDate = new DateOnly(2022, 2, 25),
            Genre = "RPG",
            Price = 59.99m,
            Platform = "PC",
            MetacriticScore = 96,
            Description = "An action role-playing game."
        };

        public static VideoGame ValidGame(string title = "Test Game") => new()
        {
            Title = title,
            Developer = "Test Developer",
            Publisher = "Test Publisher",
            ReleaseDate = new DateOnly(2023, 1, 1),
            Genre = "Action",
            Price = 29.99m,
            Platform = "PC",
            MetacriticScore = 80,
            Description = "A test game."
        };
    }
}
