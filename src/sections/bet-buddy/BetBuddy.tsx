import type { FC } from 'react';
import { LightningIcon } from 'src/components/icons/LightningIcon.tsx';

type Props = {
  userId: string;
};

const BetBuddy: FC<Props> = () => {
  return (
    <div className="bg bg-secondary hover:border-primary fixed right-4 bottom-4 cursor-pointer rounded-full p-7 transition-shadow hover:shadow-[3px_3px_10px_-0.5px_#16bdf9]">
      <LightningIcon />
    </div>
  );
};

export default BetBuddy;
