import { type FC } from 'react';
import { getMatch, useMatches } from 'src/hooks/useMatches.ts';
import MarketsSection from 'src/sections/markets/MarketsSection.tsx';

interface IMatchPageProps {
  id: number;
}

const MatchPage: FC<IMatchPageProps> = ({ id }) => {
  const { data: match } = useMatches({ select: getMatch(id) });

  return (
    <main className="flex flex-col gap-4">
      <MarketsSection match={match} />
    </main>
  );
};

export default MatchPage;
