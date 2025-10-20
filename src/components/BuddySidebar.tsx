import { type FC, useState } from 'react';
import TextInput from 'src/components/speech-to-text-input/TextInput.tsx';
import { useBetBuddy } from 'src/hooks/useBetBuddy.ts';

type Props = {
  userId: string;
};

const BuddySidebar: FC<Props> = ({ userId }) => {
  const { mutateAsync: askBuddy, data } = useBetBuddy();
  const [value, setValue] = useState('');

  const handleSubmit = (value: string) => {
    void askBuddy({ question: value, userId });
  };

  return (
    <div className="bg-secondary border-dark fixed top-15 right-0 bottom-0 z-10 w-100 border-[1px] p-2">
      <section className="flex h-full flex-col rounded-md">
        <div className="h-full grow">
          <p>tutaj są dymki</p>
        </div>
        <TextInput value={value} onChange={setValue} onSubmit={handleSubmit} />
      </section>
    </div>
  );
};

export default BuddySidebar;
