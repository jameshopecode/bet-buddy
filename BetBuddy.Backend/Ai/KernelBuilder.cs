using BetBuddy.Backend.Api.Data;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;

namespace BetBuddy.Backend.Api.Ai;

public class KernelBuilder
{
    public Kernel BuildOllamaKernel()
    {
        var builder = Kernel.CreateBuilder();
        var client = new HttpClient();
        client.BaseAddress = new Uri("http://209.38.253.188:11004");
        client.Timeout = TimeSpan.FromMinutes(3);
        builder.AddOllamaChatCompletion("gpt-oss:20b-cloud",client);
        builder.AddOllamaEmbeddingGenerator("nomic-embed-text:v1.5",client);
        
        builder.Services.AddQdrantVectorStore(
            host: "localhost",
            port: 6334,
            https: false
        );
        builder.Plugins.AddFromType<FixturesVectorStore>("Fixtures");
        var kernel = builder.Build();
        var vectorStore = kernel.GetRequiredService<QdrantVectorStore>();
        var embeddingService = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

        //Local seed for debug, real not here
        //InitializeVectorStoreAsync(vectorStore, embeddingService).Wait();
        return kernel;
    }
    
     public async Task InitializeVectorStoreAsync(QdrantVectorStore vectorStore, IEmbeddingGenerator<string, Embedding<float>> embeddingService)
    {
            // Get the car collection
            var qdrantCollection = vectorStore.GetCollection<Guid, Fixture>("fixtures");;
            
            // Ensure the collection exists
            await qdrantCollection.EnsureCollectionExistsAsync();

            // Sample car inventory
            var fixtures = new[]
            {
                new Fixture()
                {
                    Id = Guid.NewGuid(),
                    MatchId = 00123989,
                    Markets = new []{123,456,65,45,67},
                    Date = "2025-11-12",
                    Home = "Real",
                    Away = "Barcelona",
                    Description = "Match Real vs Barcenolna Date time: 2025-11-12 in La Liga competition."
                },
                new Fixture()
                {
                    Id = Guid.NewGuid(),
                    MatchId = 999866,
                    Markets = new []{659,4489,02,8634,644},
                    Date = "2025-11-12",
                    Home = "Arsenal",
                    Away = "City",
                    Description = "Match Arsenal vs City Date time: 2025-11-14 in Premier League competition."
                }
            };

            // Generate embeddings and upsert cars
            foreach (var car in fixtures)
            {
                var embedding = await embeddingService.GenerateAsync(car.Description);
                car.DescriptionEmbedding = embedding.Vector;
            }

            await qdrantCollection.UpsertAsync(fixtures);
            
            Console.WriteLine($"Initialized car inventory with {fixtures.Length} vehicles.");
    }
}