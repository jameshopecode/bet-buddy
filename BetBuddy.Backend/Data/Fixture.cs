using Microsoft.Extensions.VectorData;

namespace BetBuddy.Backend.Api.Data;

public class Fixture
{
    [VectorStoreKey]
    public Guid Id { get; set; }
    [VectorStoreData(IsFullTextIndexed = true)]
    public string Description { get; set; } = string.Empty;
    [VectorStoreData]
    public bool IsAvailable { get; set; }
    [VectorStoreVector(768)]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}