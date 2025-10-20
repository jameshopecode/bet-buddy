import type { FC } from 'react';
import { LightningIcon } from 'src/components/icons/LightningIcon.tsx';
import BuddySidebar from 'src/components/BuddySidebar.tsx';
import { cn } from 'src/utils/cn.ts';
import { ChatContextProvider } from 'src/context/ChatContextProvider.tsx';

type Props = {
  userId: string;
};

const Trigger = ({
  handleClick,
  isOpen,
}: {
  handleClick: () => void;
  isOpen: boolean;
}) => (
  <div
    className={cn(
      'bg bg-secondary hover:border-primary fixed bottom-4 z-20 cursor-pointer rounded-full p-7 transition-shadow hover:shadow-[3px_3px_10px_-0.5px_#16bdf9]',
      isOpen ? 'pointer-events-none right-75' : 'right-4'
    )}
    onClick={handleClick}
  >
    <LightningIcon />
  </div>
);

const BetBuddy: FC<Props> = ({ userId }) => {
  return (
    <ChatContextProvider>
      <BuddySidebar userId={userId} />
    </ChatContextProvider>
  );
};

export default BetBuddy;
