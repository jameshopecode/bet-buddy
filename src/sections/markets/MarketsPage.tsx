import { type FC } from 'react';
import {
  getMatchesWithMarketsMetadataFromParam,
} from 'src/model/chat.model.ts';
import { getMatchesWithMarkets, useMatches } from 'src/hooks/useMatches.ts';
import MarketsSection from 'src/sections/markets/MarketsSection.tsx';

interface IMarketsPageProps {
  metadataString: string
}

const MarketsPage: FC<IMarketsPageProps> = ({ metadataString }) => {
  const metadata = getMatchesWithMarketsMetadataFromParam(metadataString);
  const { data: matches } = useMatches({ select: getMatchesWithMarkets(metadata) });

  if (!matches) {
    return null;
  }

  return <main className="flex flex-col gap-4">
    {Object.values(matches).map(match => (
      <MarketsSection key={match.id} match={match} />
    ))}
  </main>;
};

export default MarketsPage;