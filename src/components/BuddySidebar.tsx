import { type FC, useState } from 'react';
import TextInput from 'src/components/speech-to-text-input/TextInput.tsx';
import { useBetBuddy } from 'src/hooks/useBetBuddy.ts';
import { useChatHistory } from 'src/hooks/useChatHistory.ts';
import Message from 'src/components/Message.tsx';
import type { IChatResponse } from 'src/model/chat.model.ts';

type Props = {
  userId: string;
};

const BuddySidebar: FC<Props> = ({ userId }) => {
  const { mutateAsync: askBuddy, data } = useBetBuddy();
  const [value, setValue] = useState('');

  const { messages, askQuestion, handleChatResponse } = useChatHistory(userId);

  const handleSubmit = async (value: string) => {
    setValue('');
    askQuestion(value);
    const response = await askBuddy({ question: value, userId });
    handleChatResponse(response as unknown as IChatResponse);
  };

  return (
    <div className="bg-secondary border-dark fixed top-15 right-0 bottom-0 z-10 w-100 border-[1px] p-2">
      <section className="flex h-full flex-col rounded-md">
        <div className="flex h-full grow flex-col gap-2 overflow-y-auto">
          {messages.map((message, index) => (
            <Message key={index} message={message} />
          ))}
        </div>
        <TextInput value={value} onChange={setValue} onSubmit={handleSubmit} />
      </section>
    </div>
  );
};

export default BuddySidebar;
