import { createContext } from 'react';
import type { ChatHistoryT } from 'src/model/chat.model.ts';
import { getSafeContext } from 'src/context/context.util.ts';

export type TChatContext = {
  chatHistory: ChatHistoryT;
  setChatHistory: (chatHistory: ChatHistoryT) => void;
};

export const ChatContext = createContext<TChatContext | null>(null);

ChatContext.displayName = 'ChatContext';

export const useChatContext = getSafeContext(ChatContext, 'ChatContext');
