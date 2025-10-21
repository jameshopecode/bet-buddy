using BetBuddy.Backend.Api.Dtos;
using Dapper;
using Npgsql;

namespace BetBuddy.Backend.Api.Data;

public class FixtureRepository
{
    private readonly string _connectionString;

    public FixtureRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<IEnumerable<Fixture>> GetFixtures(string query)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryAsync<Fixture>(query);
    }

    public async Task<IEnumerable<Fixture>> GetAllFixtures()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        var query = @"
            SELECT
                m.id as MatchId,
                m.start_time::text as Date,
                m.home as Home,
                m.away as Away,
                ARRAY_AGG(distinct mk.id) as Markets,
                FORMAT(
                    'Match: %s vs %s. Competition: %s. Data: %s. Home Team: %s. Away team: %s. Game type: %s. Available markets: %s',
                    m.home,
                    m.away,
                    m.competition,
                    m.start_time,
                    m.home,
                    m.away,
                    m.game,
                    STRING_AGG(distinct mk.name || ' (' || mk.market_type || ' ' || mk.id || ') ', ', ')
                ) as Description
            FROM
                matches m
            INNER JOIN
                markets mk on mk.match_id = m.id
            GROUP BY
                m.id, m.start_time, m.home, m.away, m.competition, m.game;
        ";
        return await connection.QueryAsync<Fixture>(query);
    }

    public async Task<IDictionary<long, MatchDto>> GetAllMatchesWithMarketsAndSelections()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        var sql = @"
            SELECT
                m.id, m.home, m.away, m.competition, m.start_time as startTime, m.game,
                mk.id, mk.name, mk.market_type as MarketType,
                s.id, s.name, s.odds
            FROM
                matches m
            LEFT JOIN
                markets mk ON m.id = mk.match_id
            LEFT JOIN
                selections s ON mk.id = s.market_id
            ORDER BY
                m.id, mk.id, s.id;
        ";

        var matchDictionary = new Dictionary<long, MatchDto>();

        await connection.QueryAsync<MatchDto, MarketDto, SelectionDto, MatchDto>(
            sql,
            (match, market, selection) =>
            {
                if (!matchDictionary.TryGetValue(match.Id, out var currentMatch))
                {
                    currentMatch = match;
                    currentMatch.Markets = new Dictionary<long, MarketDto>();
                    matchDictionary.Add(currentMatch.Id, currentMatch);
                }

                if (market != null)
                {
                    var currentMarket = currentMatch.Markets.ContainsKey(market.Id) ?  currentMatch.Markets[market.Id] : null;   

                    if (currentMarket == null)
                    {
                        currentMarket = market;
                        currentMarket.Selections = new List<SelectionDto>();
                        currentMatch.Markets.Add(currentMarket.Id, currentMarket);
                    }
                    
                    if (selection != null)
                    {
                        currentMarket.Selections.Add(selection);
                    }
                }

                return currentMatch;
            },
            splitOn: "id,id,id"
        );

        return matchDictionary;
    }
}
