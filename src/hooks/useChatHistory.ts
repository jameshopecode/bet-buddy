import type { ChatHistoryT, IChatResponse } from 'src/model/chat.model.ts';
import { useChatContext } from 'src/context/chat.context.ts';
import {
  getCurrentChatHistoryForUserFromLS,
  sanitizeChatResponse,
  saveChatHistoryToLS,
} from 'src/utils/chat.utils.ts';

export const useChatHistory = (userId: string) => {
  const { chatHistory, setChatHistory } = useChatContext();
  const contextChatHistoryForUser = chatHistory[userId];

  const askQuestion = (question: string) => {
    const newChatHistory = [
      ...(contextChatHistoryForUser ?? []),
      { question, answer: null, metadata: null },
    ];
    setChatHistory({
      [userId]: newChatHistory,
    });
    saveChatHistoryToLS({ [userId]: newChatHistory });
  };

  const handleChatResponse = (response: IChatResponse) => {
    const answer = sanitizeChatResponse(response);

    const currentChatHistoryForUser =
      getCurrentChatHistoryForUserFromLS(userId);

    const newChatHistory = [
      ...(currentChatHistoryForUser ?? []),
      { question: null, answer, metadata: response?.metadata || null },
    ];

    setChatHistory({
      [userId]: newChatHistory,
    });
    saveChatHistoryToLS({
      [userId]: newChatHistory,
    });
  };

  return {
    askQuestion,
    handleChatResponse,
    messages: (contextChatHistoryForUser ?? []) as ChatHistoryT[string],
  };
};
