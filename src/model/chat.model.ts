export interface IChatRequest {
  userId: string;
  question: string;
}

export interface IChatResponse {
  answer: string;
  metadata: MatchesMarketsMetadata;
}

type MatchesMarketsMetadata = Record<number, number[]>;

export type ChatHistoryT = {
  [userId: string]: [
    {
      question: string;
      answer: string;
    },
  ];
};
