namespace VideoGameCatalogue.API.Contracts
{
    public record VideoGameResponse(
        int Id,
        string Title,
        string Developer,
        string Publisher,
        DateOnly ReleaseDate,
        string Genre,
        decimal Price,
        string Platform,
        int MetacriticScore,
        string Description,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}
