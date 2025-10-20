import type { FC } from 'react';
import { LightningIcon } from 'src/components/icons/LightningIcon.tsx';
import { useEffect } from 'react';
import BuddySidebar from 'src/components/BuddySidebar.tsx';
import { useStore } from '@nanostores/react';
import { $changeSidebarOpenState, $isOpen } from 'src/stores/chat.ts';
import { cn } from 'src/utils/cn.ts';

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
  const isSidebarOpen = useStore($isOpen);

  const handleClick = () => {
    $changeSidebarOpenState(!isSidebarOpen);
    localStorage.setItem('isSidebarOpen', JSON.stringify(!isSidebarOpen));
  };

  useEffect(() => {
    $changeSidebarOpenState(
      JSON.parse(localStorage.getItem('isSidebarOpen') || 'false')
    );
  }, []);

  return (
    <>
      <Trigger isOpen={isSidebarOpen} handleClick={handleClick} />
      <BuddySidebar
        isOpen={isSidebarOpen}
        onClose={() => $changeSidebarOpenState(false)}
        userId={userId}
      />
    </>
  );
};

export default BetBuddy;
