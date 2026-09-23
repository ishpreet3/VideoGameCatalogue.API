using VideoGameCatalogue.API.Models;

namespace VideoGameCatalogue.API.Repositories
{
    public interface IVideoGameRepository
    {
        Task<IReadOnlyList<VideoGame>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<VideoGame?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<VideoGame> CreateAsync(VideoGame videoGame, CancellationToken cancellationToken = default);

        /// <summary>Saves changes made to a game previously loaded with <see cref="GetByIdAsync"/>.</summary>
        Task UpdateAsync(VideoGame videoGame, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
