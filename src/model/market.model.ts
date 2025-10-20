export interface IMarket {
  id: number;
  matchId: number;
  name: string
  type: string // Handicap / Player Props
  selections: {
    id: number
    name: string
    odds: number
  }[]
  // added by FE
  url?: string
}

export const getMarketUrl = ({
  id,
  matchId,
}: Pick<IMarket, 'id' | 'matchId'>) => `/match/${matchId}?marketId=${id}`;
