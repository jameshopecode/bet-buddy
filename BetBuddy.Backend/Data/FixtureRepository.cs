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
                    STRING_AGG(distinct mk.name || ' (' || mk.market_type || ')', ', ')
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
}
