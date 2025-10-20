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
