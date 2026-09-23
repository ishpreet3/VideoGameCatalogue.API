namespace VideoGameCatalogue.API.Domain
{
    /// <summary>
    /// The allowed values for a game's genre and platform. The API validates against these
    /// and serves them to the client, so there is a single source of truth.
    /// </summary>
    public static class Lookups
    {
        public static readonly IReadOnlyList<string> Genres =
        [
            "Action", "Action-Adventure", "Adventure", "Fighting", "Indie", "Platformer",
            "Puzzle", "Racing", "RPG", "Shooter", "Simulation", "Sports", "Strategy"
        ];

        public static readonly IReadOnlyList<string> Platforms =
        [
            "PC", "PlayStation 5", "Xbox Series X", "Nintendo Switch", "Mobile", "VR"
        ];
    }
}
