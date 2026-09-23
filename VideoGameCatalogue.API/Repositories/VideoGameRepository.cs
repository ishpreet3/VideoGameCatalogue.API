using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.API.Data;
using VideoGameCatalogue.API.Models;

namespace VideoGameCatalogue.API.Repositories
{
    public class VideoGameRepository : IVideoGameRepository
    {
        private readonly VideoGameContext _context;
        private readonly TimeProvider _timeProvider;

        public VideoGameRepository(VideoGameContext context, TimeProvider timeProvider)
        {
            _context = context;
            _timeProvider = timeProvider;
        }

        public async Task<IReadOnlyList<VideoGame>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.VideoGames
                .AsNoTracking()
                .OrderBy(vg => vg.Title)
                .ToListAsync(cancellationToken);
        }

        public async Task<VideoGame?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.VideoGames.FindAsync([id], cancellationToken);
        }

        public async Task<VideoGame> CreateAsync(VideoGame videoGame, CancellationToken cancellationToken = default)
        {
            var now = _timeProvider.GetUtcNow().UtcDateTime;
            videoGame.CreatedAt = now;
            videoGame.UpdatedAt = now;

            _context.VideoGames.Add(videoGame);
            await _context.SaveChangesAsync(cancellationToken);

            return videoGame;
        }

        public async Task UpdateAsync(VideoGame videoGame, CancellationToken cancellationToken = default)
        {
            videoGame.UpdatedAt = _timeProvider.GetUtcNow().UtcDateTime;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var deletedCount = await _context.VideoGames
                .Where(vg => vg.Id == id)
                .ExecuteDeleteAsync(cancellationToken);

            return deletedCount > 0;
        }
    }
}
