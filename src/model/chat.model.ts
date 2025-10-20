export interface IChatRequest {
  userId: string;
  question: string;
}

export interface IChatResponse {
  answer: string;
  metadata: MatchesMarketsMetadata;
}

export type ChatHistoryT = {
  [userId: string]: Array<ChatMessageT>;
};

export type ChatMessageT =
  | {
      question: string;
      answer: null;
    }
  | {
      question: null;
      answer: string;
      // TODO: any
      metadata?: any;
    };

export type MatchesMarketsMetadata = Record<number, number[]>

export const getMatchesWithMarketsUrl = (metadata: MatchesMarketsMetadata) => {
  return `/markets/${window.btoa(JSON.stringify(metadata))}`;
}

export const getMatchesWithMarketsMetadataFromParam = (param: string) => {
  const jsonString = window.atob(param);

  return JSON.parse(jsonString) as MatchesMarketsMetadata;
}