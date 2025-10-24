using System.ComponentModel;
using Google.Protobuf.Collections;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using static Qdrant.Client.Grpc.Conditions;

namespace BetBuddy.Backend.Api.Data;

public class FixturesVectorStore
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingService;
    private readonly QdrantClient qdrantClient;

    public FixturesVectorStore(IEmbeddingGenerator<string, Embedding<float>> embeddingService)
    {
        _embeddingService = embeddingService;
        qdrantClient = new QdrantClient("localhost");
    }

    [KernelFunction("SearchFixtures")]
    [Description("Search for all upcomming fixtures, matches with markets based on user requirements")]
    public async Task<FixtureResult[]> SearchFixturesAsync(
        [Description("The search query describing what kind of fixtures, matchs or markets looking for")]
        string query, 
        [Description("The sport type can be Sport or Esport")]
        string type)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        await EnsureFullTextIndexExists();
        
        var queryEmbedding = await _embeddingService.GenerateAsync(query);
        var qdrantClient = new QdrantClient("localhost");

        List<ScoredPoint> vectorResults = new List<ScoredPoint>();
        if (!string.IsNullOrEmpty(type) && type != "NA")
        {
            var vectorResults3 = await qdrantClient.SearchAsync(
                collectionName: "fixtures",
                vector: queryEmbedding.Vector,
                limit: 20,
                scoreThreshold: 0.3f
            );
            vectorResults.AddRange(vectorResults3);
        }
        else
        {
            var vectorResults2 = await qdrantClient.SearchAsync(
                collectionName: "fixtures",
                vector: queryEmbedding.Vector,
                filter: MatchText("Type", type.ToLower()),
                limit: 20,
                scoreThreshold: 0.3f
            );
            
            vectorResults.AddRange(vectorResults2);
        }

        var exactResults = await qdrantClient.SearchAsync(
            collectionName: "fixtures",
            vector: queryEmbedding.Vector,
            filter: MatchText("Description", query.ToLower()),
            limit: 10
        );

        var vectorSearchResults = ConvertToSearchResults(vectorResults);
        var exactSearchResults = ConvertToSearchResults(exactResults);

        var mergedResults = HybridSearchService.MergeAndRank(
            exactSearchResults,
            vectorSearchResults,
            query
        );

        var results = mergedResults
            .Select(r => r.Record)
            .Take(20)
            .ToList();
        watch.Stop();
        Console.WriteLine($"Hybrid search Processing time = {watch.Elapsed.Seconds}");
        return results.ToArray();
    }

    private async Task EnsureFullTextIndexExists()
    {
        try
        {
            var collectionInfo = await qdrantClient.GetCollectionInfoAsync("fixtures");

            var hasDescriptionIndex = collectionInfo.PayloadSchema
                .ContainsKey("Description");

            if (!hasDescriptionIndex)
            {
                Console.WriteLine("Creating full-text index on Description field...");
                await CreateFullTextIndex();
            }
            else
            {
                Console.WriteLine("Full-text index on Description already exists");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking index: {ex.Message}");
            await CreateFullTextIndex();
        }
    }

    private async Task CreateFullTextIndex()
    {
        try
        {
            await qdrantClient.CreatePayloadIndexAsync(
                collectionName: "fixtures",
                fieldName: "Description",
                schemaType: PayloadSchemaType.Text,
                indexParams: new PayloadIndexParams()
                {
                    TextIndexParams = new TextIndexParams
                    {
                        Tokenizer = TokenizerType.Word,
                        MinTokenLen = 2,
                        MaxTokenLen = 20,
                        Lowercase = true
                    }
                },
                wait: true
            );

            Console.WriteLine("Full-text index created successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating index: {ex.Message}");
            throw;
        }
    }

    private List<HybridSearchService.SearchResult<Fixture>> ConvertToSearchResults(
        IReadOnlyList<ScoredPoint> qdrantResults)
    {
        return qdrantResults.Select(r => new HybridSearchService.SearchResult<Fixture>
        {
            Id = r.Payload.ContainsKey("MatchId")
                ? (ulong)r.Payload["MatchId"].IntegerValue
                : 0,
            Score = r.Score,
            Record = MapToFixtureRecord(r)
        }).ToList();
    }

    private Fixture MapToFixtureRecord(ScoredPoint scoredPoint)
    {
        var payload = scoredPoint.Payload;

        return new Fixture
        {
            Id = Guid.Parse(scoredPoint.Id.Uuid),
            MatchId = payload.ContainsKey("MatchId")
                ? (int)payload["MatchId"].IntegerValue
                : 0,
            Date = payload.ContainsKey("Date")
                ? payload["Date"].StringValue
                : string.Empty,
            Home = payload.ContainsKey("Home")
                ? payload["Home"].StringValue
                : string.Empty,
            Away = payload.ContainsKey("Away")
                ? payload["Away"].StringValue
                : string.Empty,
            Markets = payload.ContainsKey("Markets")
                ? payload["Markets"].ListValue.Values.Select(x => x.IntegerValue).ToArray()
                : null,
            Description = payload.ContainsKey("Description")
                ? payload["Description"].StringValue
                : string.Empty,
            DescriptionEmbedding = null
        };
    }
}