using Microsoft.AspNetCore.Mvc;
using VideoGameCatalogue.API.Contracts;
using VideoGameCatalogue.API.Repositories;

namespace VideoGameCatalogue.API.Controllers
{
    // Invalid request bodies are rejected with a 400 ValidationProblemDetails by [ApiController],
    // and unhandled exceptions become a 500 ProblemDetails via the exception handler in Program.cs.
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class VideoGamesController : ControllerBase
    {
        private readonly IVideoGameRepository _repository;

        public VideoGamesController(IVideoGameRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VideoGameResponse>>> GetAllVideoGames(CancellationToken cancellationToken)
        {
            var games = await _repository.GetAllAsync(cancellationToken);
            return Ok(games.Select(game => game.ToResponse()));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VideoGameResponse>> GetVideoGameById(int id, CancellationToken cancellationToken)
        {
            var game = await _repository.GetByIdAsync(id, cancellationToken);
            return game is null ? NotFound() : game.ToResponse();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VideoGameResponse>> CreateVideoGame(VideoGameRequest request, CancellationToken cancellationToken)
        {
            var game = await _repository.CreateAsync(request.ToEntity(), cancellationToken);
            return CreatedAtAction(nameof(GetVideoGameById), new { id = game.Id }, game.ToResponse());
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VideoGameResponse>> UpdateVideoGame(int id, VideoGameRequest request, CancellationToken cancellationToken)
        {
            var game = await _repository.GetByIdAsync(id, cancellationToken);
            if (game is null)
                return NotFound();

            request.ApplyTo(game);
            await _repository.UpdateAsync(game, cancellationToken);

            return game.ToResponse();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVideoGame(int id, CancellationToken cancellationToken)
        {
            var deleted = await _repository.DeleteAsync(id, cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
    }
}
