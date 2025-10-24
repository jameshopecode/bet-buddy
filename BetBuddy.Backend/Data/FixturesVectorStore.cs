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

    [KernelFunction("SearchFixturesByType")]
    [Description("Search fixtures, matches, markets, markets, competitions for user type requirements")]
    public async Task<FixtureResult[]> SearchFixturesAsyncByType([Description("examples: barcelona vs real, champions league, availiable markets include: handicap etc.")]string userQuery,[Description("Sport/Esport")] string typeRequirement)
    {
        var queryEmbedding = await _embeddingService.GenerateAsync(userQuery);
        var qdrantClient = new QdrantClient("localhost");

        var vectorResults  = await qdrantClient.SearchAsync(
            collectionName: "fixtures",
            vector: queryEmbedding.Vector,
            limit: 20,
            filter: MatchText("Type", typeRequirement),
            scoreThreshold: 0.45f
        );
        
        var vectorSearchResults = ConvertToSearchResults(vectorResults);

        var results = vectorSearchResults
            .Select(r => new FixtureResult()
            {
                Description =r.Record.Description,
                Markets = r.Record.Markets,
                MatchId = r.Record.MatchId
            })
            .Take(20)
            .ToList();
        
        return results.ToArray();
    }

    [KernelFunction("SearchFixtures")]
    [Description("Search fixtures, matches, markets, markets, competitions")]
    public async Task<FixtureResult[]> SearchFixturesAsync(
        [Description(@"The search query describing what kind of fixtures, matchs, team, competion or markets shoulb found semanticaly. For example:
1. barcelona match in LaLiga
2. Esport match beetwen Heroic vs Mouze
3. availiable marketes incloude handicap
4. [COMPETITION: CHAMPIONS LEAGUE]
5. [TYPE: Esport]")]
        string query)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        await EnsureFullTextIndexExists();
        
        var queryEmbedding = await _embeddingService.GenerateAsync(query);
        var qdrantClient = new QdrantClient("localhost");

        var vectorResults  = await qdrantClient.SearchAsync(
                collectionName: "fixtures",
                vector: queryEmbedding.Vector,
                limit: 20,
                scoreThreshold: 0.45f
            );

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