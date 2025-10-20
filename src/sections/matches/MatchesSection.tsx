import  { type FC } from 'react';
import { useMatches } from 'src/hooks/useMatches.ts';
import MatchRow from 'src/sections/matches/components/MatchRow.tsx';

const MatchesSection: FC = () => {
  const { data: matches } = useMatches();

  return (
    <main className="flex flex-col gap-4">
      <h1 className="text-lg font-bold text-center">Bet on matches</h1>
      {Object.values(matches ?? {}).map(match => (
        <MatchRow key={match.id} match={match} />
      ))}
    </main>
  );
};

export default MatchesSection;