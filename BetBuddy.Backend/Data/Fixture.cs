using Microsoft.Extensions.VectorData;

namespace BetBuddy.Backend.Api.Data;

public class Fixture
{
    [VectorStoreKey]
    public Guid Id { get; set; }
    
    [VectorStoreData]
    public int MatchId { get; set; }
    
    [VectorStoreData]
    public string Date { get; set; } = string.Empty;
    
    [VectorStoreData]
    public string Home { get; set; } = string.Empty;
    
    [VectorStoreData]
    public string Away { get; set; } = string.Empty;
    
    [VectorStoreData(IsFullTextIndexed = true, IsIndexed = true)]
    public string Type { get; set; } = string.Empty; // "Football", "Esports", "Basketball"
    
    [VectorStoreData(IsIndexed = true)]
    public string Game { get; set; } = string.Empty; // "DOTA 2", "CS:GO", "N/A" for traditional sports
    
    [VectorStoreData(IsIndexed = true)]
    public string Competition { get; set; } = string.Empty;

    [VectorStoreData] public long[]? Markets { get; set; } = null;
    
    [VectorStoreData(IsFullTextIndexed = true, IsIndexed = true)]
    public string Description { get; set; } = string.Empty;
    
    [VectorStoreVector(768, IndexKind = IndexKind.Hnsw)]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}

public class FixtureResult
{
    public int MatchId { get; set; }
    public long[]? Markets { get; set; } = null;
    public string Description { get; set; } = string.Empty;
}