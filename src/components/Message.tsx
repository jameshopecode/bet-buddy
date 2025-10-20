import type { FC } from 'react';
import type { ChatMessageT } from 'src/model/chat.model.ts';
import MetadataConsumer from 'src/components/MetadataConsumer.tsx';

type Props = {
  message: ChatMessageT;
};

const Question = ({ question }: { question: string }) => {
  return (
    <div className="flex justify-end">
      <p className="bg-info w-4/5 rounded-sm p-2 text-white">{question}</p>
    </div>
  );
};

const Answer = ({ message }: { message: ChatMessageT }) => {
  const { answer } = message;

  const hasMetadata =
    'metadata' in message &&
    !!message.metadata &&
    Object.keys(message.metadata || {}).length > 0;

  return (
    <div className="flex flex-col">
      <p className="bg-dark w-4/5 rounded-sm p-2 text-white">{answer}</p>
      {hasMetadata && <MetadataConsumer message={message} />}
    </div>
  );
};

const Message: FC<Props> = ({ message }) => {
  const { question, answer } = message;

  const isQuestion = question !== null;
  const isAnswer = answer !== null;

  return (
    <>
      {isQuestion && <Question question={question} />}
      {isAnswer && <Answer message={message} />}
    </>
  );
};

export default Message;
