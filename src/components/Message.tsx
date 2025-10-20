import type { FC } from 'react';
import type { ChatMessageT } from 'src/model/chat.model.ts';

type Props = {
  message: ChatMessageT;
};

const Question = ({ question }: { question: string }) => {
  return (
    <div className="flex justify-end">
      <p className="bg-primary w-4/5 rounded-sm p-2 text-white">{question}</p>
    </div>
  );
};

const Answer = ({ answer }: { answer: string }) => {
  return (
    <div className="flex">
      <p className="bg-dark w-4/5 rounded-sm p-2 text-white">{answer}</p>
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
      {isAnswer && <Answer answer={answer} />}
    </>
  );
};

export default Message;
