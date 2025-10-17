interface IChatRequest {
  userId: string
  question: string
}

interface IChatResponse {
  answer: string
  metadata: MatchesMarketsMetadata
}

type MatchesMarketsMetadata = Record<number, number[]>