using BetBuddy.Backend.Api.Ai;
using BetBuddy.Backend.Api.Data;
using BetBuddy.Backend.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;

namespace BetBuddy.Backend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuddyController : ControllerBase
{
    private readonly ILogger<BuddyController> _logger;
    private readonly IBetBuddyAgent  _agent;
    private readonly FixtureRepository  _fixtureRepository;
    private readonly Kernel _kernel;
    private readonly QdrantVectorStore vectorStore;
    private readonly IEmbeddingGenerator<string, Embedding<float>> embeddingService;
    public BuddyController(ILogger<BuddyController> logger, IBetBuddyAgent agent, FixtureRepository fixtureRepository, Kernel kernel)
    {
        _logger = logger;
        _agent = agent;
        _fixtureRepository = fixtureRepository;
        _kernel = kernel;
        
        vectorStore = kernel.GetRequiredService<QdrantVectorStore>();
        embeddingService = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
    }

    [HttpGet("check")]
    public async Task<IActionResult> Get()
    {
        var allFixtures = await _fixtureRepository.GetAllFixtures();
        await InitializeVectorStoreAsync(allFixtures);
        
        return await Task.FromResult(Ok("Seed vectors ok"));
    }
    
    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] InputModel model)
    {
        if (model is null || string.IsNullOrWhiteSpace(model.Question))
        {
            return BadRequest(new { error = "Invalid payload. 'input' is required." });
        }

        var response = await _agent.Interact(model.Question, model.UserId);
        return Ok(response);
    }
    
    public async Task InitializeVectorStoreAsync(IEnumerable<Fixture> fixtures)
    {
        var qdrantCollection = vectorStore.GetCollection<Guid, Fixture>("fixtures");;
            
   
        await qdrantCollection.EnsureCollectionExistsAsync();
        
        
        foreach (var fixture in fixtures)
        {
            var embedding = await embeddingService.GenerateAsync(fixture.Description);
            fixture.DescriptionEmbedding = embedding.Vector;
            fixture.Id = Guid.NewGuid();
        }

        await qdrantCollection.UpsertAsync(fixtures);
            
        Console.WriteLine($"Initialized car inventory with {fixtures?.Count()} vehicles.");
    }
}