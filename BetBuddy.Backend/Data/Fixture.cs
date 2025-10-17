using Microsoft.Extensions.VectorData;

namespace BetBuddy.Backend.Api.Data;

public class Fixture
{
    [VectorStoreKey]
    public Guid Id { get; set; }
    
    [VectorStoreData]
    public int MatchId { get; set; }
    
    [VectorStoreData(IsFullTextIndexed = true)]
    public string Date { get; set; } = string.Empty;
    
    [VectorStoreData(IsFullTextIndexed = true)]
    public string Home { get; set; } = string.Empty;
    
    [VectorStoreData(IsFullTextIndexed = true)]
    public string Away { get; set; } = string.Empty;

    [VectorStoreData] public long[]? Markets { get; set; } = null;
    
    [VectorStoreData(IsFullTextIndexed = true)]
    public string Description { get; set; } = string.Empty;
    
    
    [VectorStoreVector(768)]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}