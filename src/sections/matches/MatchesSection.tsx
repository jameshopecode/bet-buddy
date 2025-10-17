import startCase from "lodash-es/startCase";
import type { FC } from 'react';
import { useMatches } from 'src/hooks/useMatches.ts';

interface IMatchesSectionProps {
  pathname: string
}

const MatchesSection: FC<IMatchesSectionProps> = ({ pathname }) => {
  const page = startCase(pathname);
  const { matches } = useMatches({ pageSize: 100 });

  console.log("MatchesSection", matches);

  return <main className="grid">
      <h1>{page} betting</h1>
      <pre>{JSON.stringify(matches)}</pre>
  </main>
};

export default MatchesSection;