import type { IMarket } from 'src/model/market.model.ts';

export interface IMatch {
  id: number
  home: string
  away: string
  competition: string
  startTime: string
  game: string // cs2 / football
  markets: {
    [marketId: number]: IMarket
  }
  // added by FE
  url: string
}

// match team A vs B
// market odds 1 vs 2

export const getMatchUrl = (id: number) => `/match/${id}`;

export type MatchesResponse = {
  [matchId: number]: IMatch
}