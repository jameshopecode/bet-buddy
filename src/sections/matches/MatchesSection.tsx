import startCase from "lodash-es/startCase";
import type { FC } from 'react';
import { useMatches } from 'src/hooks/useMatches.ts';
import TextInput from 'src/components/speech-to-text-input/TextInput.tsx';

interface IMatchesSectionProps {
  pathname: string
}

const MatchesSection: FC<IMatchesSectionProps> = ({ pathname }) => {
  const page = startCase(pathname);
  const { matches } = useMatches({ pageSize: 100 });

  console.log("MatchesSection", matches);

  return <main className="grid">
      <h1>{page}</h1>
      <TextInput />
  </main>
};

export default MatchesSection;