import { useQuery } from '@tanstack/react-query';
import { queryClient } from 'src/core/query/query-client.ts';
import { getMatchUrl, type MatchesResponse } from 'src/model/match.model.ts';
import { mapValues } from 'lodash-es';
import { getMarketUrl } from 'src/model/market.model.ts';

interface IUseMatchesInput {
  pageSize: number
}

export function useMatches({ pageSize = 10 }: IUseMatchesInput) {
  const { data } = useQuery<MatchesResponse>({
    queryKey: ["matches", pageSize],
    queryFn: async () => {
      return {
        [1]: {
          id: 1,
          teams: { home: "team A", away: "team B" },
          competition: "Competition",
          startTime: new Date().toISOString(),
          game: "CS 2",
          markets: {
            [1]: {
              id: 1,
              matchId: 1,
              name: "Match winner",
              type: "Winner",
              selections: [
                {
                  id: 1,
                  name: "Team A",
                  odds: 1.2,
                },
                {
                  id: 2,
                  name: "Team B",
                  odds: 1.2,
                },
              ],
              // added by FE
              url: getMarketUrl({ id: 1, matchId: 1 }),
            },
          },
          url: getMatchUrl(1),
        }
      }
      const matches: MatchesResponse = await fetch("/api/fixture/all")
        .then(response => response.json());

      return mapValues(matches, match => {
        match.url = getMatchUrl(match.id);
        match.markets = mapValues(match.markets, mkt => {
          mkt.url = getMarketUrl(mkt);
          return mkt;
        })

        return match;
      })
    },
  }, queryClient);

  return { matches: data };
}