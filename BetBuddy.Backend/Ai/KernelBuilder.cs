using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;

namespace BetBuddy.Backend.Api.Ai;

public class KernelBuilder
{
    public Kernel BuildOllamaKernel()
    {
        var builder = Kernel.CreateBuilder();
        var client = new HttpClient();
        client.BaseAddress = new Uri("http://localhost:11434");
        client.Timeout = TimeSpan.FromMinutes(3);
        builder.AddOllamaChatCompletion("gpt-oss:20b-cloud",client);
        builder.AddOllamaEmbeddingGenerator("nomic-embed-text:v1.5",client);
        
        builder.Services.AddQdrantVectorStore(
            host: "localhost",
            port: 6334,
            https: false
        );

       // builder.Plugins.AddFromType<VectorStore>("Inventory");
        
        var kernel = builder.Build();

        return kernel;
    }
}