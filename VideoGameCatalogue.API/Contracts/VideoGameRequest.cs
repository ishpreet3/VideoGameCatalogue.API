using System.ComponentModel.DataAnnotations;

namespace VideoGameCatalogue.API.Contracts
{
    /// <summary>
    /// The body of a create or update request. Id and timestamps are deliberately absent,
    /// so clients cannot set them.
    /// </summary>
    public record VideoGameRequest
    {
        [Required, MaxLength(200)]
        public required string Title { get; init; }

        [Required, MaxLength(150)]
        public required string Developer { get; init; }

        [Required, MaxLength(150)]
        public required string Publisher { get; init; }

        public required DateOnly ReleaseDate { get; init; }

        [Required, MaxLength(100), AllowedGenre]
        public required string Genre { get; init; }

        [Range(0, 99_999_999.99)]
        public required decimal Price { get; init; }

        [Required, MaxLength(100), AllowedPlatform]
        public required string Platform { get; init; }

        [Range(0, 100)]
        public required int MetacriticScore { get; init; }

        [MaxLength(1000)]
        public string? Description { get; init; }
    }
}
