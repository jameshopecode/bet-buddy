interface IMatch {
  id: number
  teams: { home: string, away: string }
  competition: string
  startTime: string
  game: string // cs2 / football
  // added by FE
  url?: string
}

// match team A vs B
// market odds 1 vs 2

export const getMatchUrl = (id: number) => `/match/${id}`;