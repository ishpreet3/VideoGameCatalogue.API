using VideoGameCatalogue.API.Models;

namespace VideoGameCatalogue.API.Contracts
{
    public static class VideoGameMappings
    {
        public static VideoGame ToEntity(this VideoGameRequest request)
        {
            var game = new VideoGame();
            request.ApplyTo(game);
            return game;
        }

        public static void ApplyTo(this VideoGameRequest request, VideoGame game)
        {
            game.Title = request.Title;
            game.Developer = request.Developer;
            game.Publisher = request.Publisher;
            game.ReleaseDate = request.ReleaseDate;
            game.Genre = request.Genre;
            game.Price = request.Price;
            game.Platform = request.Platform;
            game.MetacriticScore = request.MetacriticScore;
            game.Description = request.Description ?? string.Empty;
        }

        public static VideoGameResponse ToResponse(this VideoGame game) => new(
            game.Id,
            game.Title,
            game.Developer,
            game.Publisher,
            game.ReleaseDate,
            game.Genre,
            game.Price,
            game.Platform,
            game.MetacriticScore,
            game.Description,
            game.CreatedAt,
            game.UpdatedAt);
    }
}
