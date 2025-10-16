using BetBuddy.Backend.Api.Ai;
using BetBuddy.Backend.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BetBuddy.Backend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuddyController : ControllerBase
{
    private readonly ILogger<BuddyController> _logger;
    private readonly IBetBuddyAgent  _agent;
    
    public BuddyController(ILogger<BuddyController> logger, IBetBuddyAgent agent)
    {
        _logger = logger;
        _agent = agent;
    }

    [HttpGet("check")]
    public async Task<IActionResult> Get()
    {
        return await Task.FromResult(Ok("Ok app"));
    }
    
    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] InputModel model)
    {
        if (model is null || string.IsNullOrWhiteSpace(model.Input))
        {
            return BadRequest(new { error = "Invalid payload. 'input' is required." });
        }

        var response = await _agent.Interact(model.Input);
        return Ok(new { response = response });
    }
}