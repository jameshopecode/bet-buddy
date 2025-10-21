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
        client.BaseAddress = new Uri("http://ollama-server:11434");
        client.Timeout = TimeSpan.FromMinutes(3);
        builder.AddOllamaChatCompletion("gpt-oss:20b-cloud",client);
        builder.AddOllamaEmbeddingGenerator("nomic-embed-text:v1.5",client);
        
        builder.Services.AddQdrantVectorStore(
            host: "qdrant",
            port: 6334,
            https: false
        );
        builder.Plugins.AddFromType<FixturesVectorStore>("Fixtures");
        var kernel = builder.Build();


        //Local seed for debug, real not here
        //InitializeVectorStoreAsync(vectorStore, embeddingService).Wait();
        return kernel;
    }
    
    
}