import { useQuery } from '@tanstack/react-query';
import { queryClient } from 'src/core/query/query-client.ts';
import { getMatchUrl, type MatchesResponse } from 'src/model/match.model.ts';
import { mapValues, pick } from 'lodash-es';
import { useCallback } from 'react';
import type { MatchesMarketsMetadata } from 'src/model/chat.model.ts';

interface IUseMatchesInput<R = MatchesResponse> {
  select?(response: MatchesResponse): R
}

export const getMatch = (id: number) => (response: MatchesResponse) => {
  return response?.[id];
};

export const getMatchesWithMarkets = (ids: MatchesMarketsMetadata) => useCallback((response: MatchesResponse) => {
  const filteredResponse: MatchesResponse = {};

  for (const [matchIdStr, marketIds] of Object.entries(ids ?? {})) {
    const matchId = Number(matchIdStr);

    if (!response[matchId]) continue;

    filteredResponse[matchId] = response[matchId];
    filteredResponse[matchId].markets = pick(response[matchId].markets, marketIds);
  }

  return filteredResponse;
}, [ids])

export function useMatches<R = MatchesResponse>({ select }: IUseMatchesInput<R> = {}) {
  const { data } = useQuery({
    queryKey: ["matches"],
    queryFn: async () => {
      return {
        [1]: {
          id: 1,
          home: "team A",
          away: "team B",
          competition: "Competition",
          startTime: new Date().toISOString(),
          game: "CS 2",
          markets: {
            [1]: {
              id: 1,
              matchId: 1,
              name: "Match winner",
              marketType: "Winner",
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
            },
          },
          url: getMatchUrl(1),
        }
      }
      const matches: MatchesResponse = await fetch("/api/fixture/all")
        .then(response => response.json());

      return mapValues(matches, match => {
        match.url = getMatchUrl(match.id);

        return match;
      })
    },
    select,
  }, queryClient);

  return { data: data as R };
}