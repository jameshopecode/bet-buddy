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
    
     public async Task InitializeVectorStoreAsync(QdrantVectorStore vectorStore, IEmbeddingGenerator<string, Embedding<float>> embeddingServicef,mgkf)
    {
            // Get the car collection
            var carCollection = _carRepository;
            
            // Ensure the collection exists
            await carCollection.EnsureCollectionExistsAsync();

            // Sample car inventory
            var cars = new[]
            {
                new Car
                {
                    Id = Guid.NewGuid(),
                    Make = "Toyota",
                    Model = "Camry",
                    Year = 2023,
                    Price = 28500,
                    Color = "Blue",
                    IsAvailable = true,
                    Description = "Reliable midsize sedan with excellent fuel economy, advanced safety features, and spacious interior perfect for daily commuting."
                },
                new Car
                {
                    Id = Guid.NewGuid(),
                    Make = "Honda",
                    Model = "Accord",
                    Year = 2024,
                    Price = 31200,
                    Color = "White",
                    IsAvailable = true,
                    Description = "Premium sedan with hybrid powertrain, leather seats, and cutting-edge infotainment system ideal for business professionals."
                },
                new Car
                {
                    Id = Guid.NewGuid(),
                    Make = "BMW",
                    Model = "X5",
                    Year = 2023,
                    Price = 62800,
                    Color = "Black",
                    IsAvailable = true,
                    Description = "Luxury SUV with powerful engine, premium interior, and advanced driver assistance perfect for families who want performance."
                },
                new Car
                {
                    Id = Guid.NewGuid(),
                    Make = "Tesla",
                    Model = "Model 3",
                    Year = 2024,
                    Price = 42990,
                    Color = "Red",
                    IsAvailable = false,
                    Description = "Electric performance sedan with autopilot, supercharging capability, and minimal maintenance costs for eco-conscious drivers."
                },
                new Car
                {
                    Id = Guid.NewGuid(),
                    Make = "Ford",
                    Model = "F-150",
                    Year = 2023,
                    Price = 38500,
                    Color = "Silver",
                    IsAvailable = true,
                    Description = "Full-size pickup truck with towing capacity, rugged build quality, and versatile bed perfect for work and recreation."
                }
            };

            // Generate embeddings and upsert cars
            foreach (var car in cars)
            {
                var embedding = await _embeddingService.GenerateAsync(car.Description);
                car.DescriptionEmbedding = embedding.Vector;
            }

            await carCollection.UpsertAsync(cars);
            
            Console.WriteLine($"Initialized car inventory with {cars.Length} vehicles.");
    }
}