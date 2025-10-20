using BetBuddy.Backend.Api.Ai;
using Microsoft.AspNetCore.Mvc;

namespace BetBuddy.Backend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    private readonly IBetBuddyAgent  _agent;

    public DataController(IBetBuddyAgent agent)
    {
        _agent = agent;
    }

    [HttpPost("cleanup")]
    public IActionResult Cleanup()
    {
        _agent.Cleanup();
        return Ok();
    }
}