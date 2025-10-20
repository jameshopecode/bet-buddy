import { useState, type FC, type ReactNode, useEffect } from 'react';

import type { ChatHistoryT } from 'src/model/chat.model.ts';
import { ChatContext } from 'src/context/chat.context.ts';

export const ChatContextProvider: FC<{
  children: ReactNode;
}> = ({ children }) => {
  const [chatHistory, setChatHistory] = useState<ChatHistoryT>({});

  useEffect(() => {
    const chatHistory = JSON.parse(localStorage.getItem('chatHistory') || '{}');
    setChatHistory(chatHistory);
  }, []);

  return (
    <ChatContext.Provider
      value={{
        chatHistory,
        setChatHistory,
      }}
    >
      {children}
    </ChatContext.Provider>
  );
};
