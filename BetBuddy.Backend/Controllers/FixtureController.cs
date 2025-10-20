using BetBuddy.Backend.Api.Data;
using BetBuddy.Backend.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BetBuddy.Backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FixtureController : ControllerBase
    {
        private readonly FixtureRepository _fixtureRepository;

        public FixtureController(FixtureRepository fixtureRepository)
        {
            _fixtureRepository = fixtureRepository;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<MatchDto>>> All()
        {
            var matches = await _fixtureRepository.GetAllMatchesWithMarketsAndSelections();
            return Ok(matches);
        }
    }
}
