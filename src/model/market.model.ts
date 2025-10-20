export interface IMarket {
  id: number;
  matchId: number;
  name: string
  marketType: string | null // Handicap / Player Props
  selections: {
    id: number
    name: string
    odds: number
  }[]
}
