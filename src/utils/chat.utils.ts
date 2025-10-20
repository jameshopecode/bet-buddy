import {
  type ChatHistoryT,
  getMatchesWithMarketsUrl,
  type IChatResponse,
} from 'src/model/chat.model.ts';

export const saveChatHistoryToLS = (chatHistory: ChatHistoryT) => {
  localStorage.setItem('chatHistory', JSON.stringify(chatHistory));
};

export const getCurrentChatHistoryForUserFromLS = (userId: string) => {
  const chatHistory = JSON.parse(localStorage.getItem('chatHistory') || '{}');
  return chatHistory[userId];
};

export const sanitizeChatResponse = (response: IChatResponse) => {
  const { answer = '' } = response;

  // TODO: More checks to see if BE answer is correct
  const isBEAnswerWrong = !answer;

  if (isBEAnswerWrong) {
    return 'I’m sorry, but I can only help with fixtures, betting, or gambling questions.';
  }
  return answer;
};

export const tryRedirectingToMarketsPage = (response: IChatResponse) => {
  const hasMetadata =
    'metadata' in response &&
    !!response.metadata &&
    Object.keys(response.metadata || {}).length > 0;

  if (!hasMetadata) {
    return;
  }

  const { metadata } = response;
  const url = getMatchesWithMarketsUrl(metadata);

  window.location.href = url;
};
