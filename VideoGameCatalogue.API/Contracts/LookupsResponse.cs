namespace VideoGameCatalogue.API.Contracts
{
    public record LookupsResponse(IReadOnlyList<string> Genres, IReadOnlyList<string> Platforms);
}
