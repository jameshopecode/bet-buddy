import { type FC, useEffect, useRef, useState } from 'react';
import TextInput from 'src/components/speech-to-text-input/TextInput.tsx';
import { useBetBuddy } from 'src/hooks/useBetBuddy.ts';
import { useChatHistory } from 'src/hooks/useChatHistory.ts';
import Message from 'src/components/Message.tsx';
import type { IChatResponse } from 'src/model/chat.model.ts';
import Loader from 'src/components/Loader.tsx';
import { tryRedirectingToMarketsPage } from 'src/utils/chat.utils.ts';

type Props = {
  userId: string;
};

const BuddySidebar: FC<Props> = ({ userId }) => {
  const { mutateAsync: askBuddy, isPending } = useBetBuddy();
  const [value, setValue] = useState('');
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const { messages, askQuestion, handleChatResponse } = useChatHistory(userId);

  const scrollDown = () => {
    setTimeout(() => {
      messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }, 1);
  };

  const handleSubmit = async (value: string) => {
    try {
      setValue('');
      askQuestion(value);
      scrollDown();
      const response = await askBuddy({ question: value, userId });
      handleChatResponse(response as unknown as IChatResponse);
      tryRedirectingToMarketsPage(response);
    } catch (e) {
      console.log('error: ', e);
      handleChatResponse({
        answer:
          'I’m sorry, but I can only help with fixtures, betting, or gambling questions right now. Try again later.',
      } as unknown as IChatResponse);
    } finally {
      scrollDown();
    }
  };

  // This effect initially scrolls down to the bottom of the chat
  useEffect(() => {
    scrollDown();
  }, []);

  return (
    <div className="bg-secondary border-dark fixed top-15 right-0 bottom-0 z-10 w-100 border-[1px] p-2">
      <section className="flex h-full flex-col rounded-md">
        <div className="flex h-full grow flex-col gap-4 overflow-y-auto">
          {messages.map((message, index) => (
            <Message key={index} message={message} />
          ))}
          {isPending && <Loader />}
          <div id="messages-bottom-anchor" ref={messagesEndRef} />
        </div>
        <TextInput
          value={value}
          onChange={setValue}
          onSubmit={handleSubmit}
          isPending={isPending}
        />
      </section>
    </div>
  );
};

export default BuddySidebar;
