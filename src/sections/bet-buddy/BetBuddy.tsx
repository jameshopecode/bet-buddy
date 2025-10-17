import type { FC } from 'react';
import { LightningIcon } from 'src/components/icons/LightningIcon.tsx';
import { useBetBuddy } from 'src/hooks/useBetBuddy.ts';

type Props = {
  userId: string;
};

const BetBuddy: FC<Props> = ({ userId }) => {
  const { mutateAsync, data } = useBetBuddy();

  // TODO: USUNĄĆ CONSOLE.LOG
  console.log('data: ', data);

  return (
    <div className="bg bg-secondary hover:border-primary fixed right-4 bottom-4 cursor-pointer rounded-full p-7 transition-shadow hover:shadow-[3px_3px_10px_-0.5px_#16bdf9]">
      <LightningIcon />
      <button
        className="absolute right-0 bottom-20 left-0 z-10 flex items-center justify-center"
        onClick={() => mutateAsync({ question: 'How are you?', userId })}
      >
        <span className="bg-primary text-white">Ask a question</span>
      </button>
    </div>
  );
};

export default BetBuddy;
