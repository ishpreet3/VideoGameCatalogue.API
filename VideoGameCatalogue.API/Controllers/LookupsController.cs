using Microsoft.AspNetCore.Mvc;
using VideoGameCatalogue.API.Contracts;
using VideoGameCatalogue.API.Domain;

namespace VideoGameCatalogue.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class LookupsController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<LookupsResponse> GetLookups() => new LookupsResponse(Lookups.Genres, Lookups.Platforms);
    }
}
