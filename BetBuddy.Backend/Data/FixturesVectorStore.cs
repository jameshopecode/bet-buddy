using System.ComponentModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;

namespace BetBuddy.Backend.Api.Data;

public class FixturesVectorStore
{
    private readonly VectorStoreCollection<Guid, Fixture> _carCollection;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingService;

    public FixturesVectorStore(QdrantVectorStore vectorStore, IEmbeddingGenerator<string, Embedding<float>> embeddingService)
    {
        _carCollection = vectorStore.GetCollection<Guid, Fixture>("fixtures");
        _embeddingService = embeddingService;
    }
    
    [KernelFunction("SearchFixtures")]
    [Description("Search for all upcomming fixtures, matches with markets based on user requirements")]
    public async Task<Fixture[]> SearchFixturesAsync(
        [Description("The search query describing what kind of fixtures, matchs or markets looking for")]
        string query)
    {
        // Generate embedding for the search query
        var queryEmbedding = await _embeddingService.GenerateAsync(query);

        // Perform vector search
 
        var searchOptions = new VectorSearchOptions<Fixture>
        {
            VectorProperty = m => m.DescriptionEmbedding,
            IncludeVectors = false
        };


        var searchResults =  _carCollection.SearchAsync(queryEmbedding,20, searchOptions);

        var results = new List<Fixture>();
        await foreach (var result in searchResults)
        {
            var fixture = result.Record;
            results.Add(fixture);
        }

        return results.ToArray();
    }
}