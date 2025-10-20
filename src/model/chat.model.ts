
interface IChatRequest {
  userId: string
  question: string
}

interface IChatResponse {
  answer: string
  metadata: MatchesMarketsMetadata
}

export type MatchesMarketsMetadata = Record<number, number[]>

export const getMatchesWithMarketsUrl = (metadata: MatchesMarketsMetadata) => {
  return `/markets/${window.btoa(JSON.stringify(metadata))}`;
}

export const getMatchesWithMarketsMetadataFromParam = (param: string) => {
  const jsonString = window.atob(param);

  return JSON.parse(jsonString) as MatchesMarketsMetadata;
}