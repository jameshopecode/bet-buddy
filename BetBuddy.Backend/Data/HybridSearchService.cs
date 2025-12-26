namespace BetBuddy.Backend.Api.Data;


public static class HybridSearchService
{
    private const int DefaultK = 60;
    
    public class SearchResult<T>
    {
        public T Record { get; set; }
        public float Score { get; set; }
        public ulong Id { get; set; }
    }
    
    public static List<SearchResult<FixtureResult>> MergeAndRank(
        IEnumerable<SearchResult<Fixture>> exactResults,
        IEnumerable<SearchResult<Fixture>> vectorResults,
        string query,
        double exactWeight = 4, 
        double vectorWeight = 1.0,
        int k = DefaultK)
    {

        var fusedScores = new Dictionary<ulong, (Fixture Record, double RRFScore)>();
        
        var exactList = exactResults.ToList();
        for (int rank = 0; rank < exactList.Count; rank++)
        {
            var result = exactList[rank];
            var rrfScore = exactWeight * result.Score / (k + rank + 1); 
            
            if (!fusedScores.ContainsKey(result.Id))
            {
                fusedScores[result.Id] = (result.Record, rrfScore);
            }
            else
            {
                var existing = fusedScores[result.Id];
                fusedScores[result.Id] = (existing.Record, existing.RRFScore + rrfScore);
            }
        }
        
 
        var vectorList = vectorResults.ToList();
        for (int rank = 0; rank < vectorList.Count; rank++)
        {
            var result = vectorList[rank];
            var rrfScore = vectorWeight  * result.Score / (k + rank + 1);
            
            if (!fusedScores.ContainsKey(result.Id))
            {
                fusedScores[result.Id] = (result.Record, rrfScore);
            }
            else
            {
                var existing = fusedScores[result.Id];
                fusedScores[result.Id] = (existing.Record, existing.RRFScore + rrfScore);
            }
        }

        var top1score = fusedScores.OrderByDescending(kvp => kvp.Value.RRFScore).First();
        return fusedScores
            .OrderByDescending(kvp => kvp.Value.RRFScore)
            .Where(x=>x.Value.RRFScore >= top1score.Value.RRFScore / 2)
            .Select(kvp => new SearchResult<FixtureResult>
            {
                Id = kvp.Key,
                Record = new FixtureResult()
                {
                    Description = kvp.Value.Record.Description,
                    Markets = kvp.Value.Record.Markets,
                    MatchId = kvp.Value.Record.MatchId
                },
                Score = (float)kvp.Value.RRFScore
            })
            .ToList();
    }
}