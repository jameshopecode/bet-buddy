import { atom, computed, map, deepMap } from 'nanostores';
import type {
  ChatHistoryT,
  IChatRequest,
  IChatResponse,
} from 'src/model/chat.model.ts';

type MessageT = IChatRequest | IChatResponse;

export const $isOpen = atom(false);

export const $chatHistory = deepMap<ChatHistoryT>({});

export const $changeSidebarOpenState = (isOpen: boolean) => {
  $isOpen.set(isOpen);
};

export const $addMessageToChatHistory = (userId: string, message: MessageT) => {
  const userChatHistory = $chatHistory.get()[userId];

  // TODO: USUNĄĆ CONSOLE.LOG
  console.log('userChatHistory: ', userChatHistory);
  // $chatHistory.set({
  //   ...$chatHistory.get(),
  // });
};
