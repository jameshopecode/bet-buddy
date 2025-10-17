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
        var kernel = builder.Build();
        var vectorStore = kernel.GetRequiredService<QdrantVectorStore>();
        var embeddingService = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
        builder.Plugins.AddFromObject(new FixturesVectorStore(vectorStore, embeddingService));
        
        
        return kernel;
    }
    
     public async Task InitializeVectorStoreAsync(QdrantVectorStore vectorStore, IEmbeddingGenerator<string, Embedding<float>> embeddingService)
    {
            // Get the car collection
            var qdrantCollection = vectorStore.GetCollection<Guid, Fixture>("Fixtures");;
            
            // Ensure the collection exists
            await qdrantCollection.EnsureCollectionExistsAsync();

            // Sample car inventory
            var cars = new[]
            {
                new Fixture()
                {
                    Id = Guid.NewGuid(),
                    IsAvailable = true,
                    Description = "Reliable midsize sedan with excellent fuel economy, advanced safety features, and spacious interior perfect for daily commuting."
                },
             
            };

            // Generate embeddings and upsert cars
            foreach (var car in cars)
            {
                var embedding = await embeddingService.GenerateAsync(car.Description);
                car.DescriptionEmbedding = embedding.Vector;
            }

            await qdrantCollection.UpsertAsync(cars);
            
            Console.WriteLine($"Initialized car inventory with {cars.Length} vehicles.");
    }
}