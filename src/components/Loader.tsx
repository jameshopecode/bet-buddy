import type { FC } from 'react';

type Props = {
  className?: string;
};

const Loader: FC<Props> = ({ className = '' }) => {
  return (
    <span
      className={`loader animate-flash relative mt-4 ml-8 h-4 w-4 rounded-full bg-white ${className} scale-75`}
      aria-hidden="true"
    />
  );
};

export default Loader;
