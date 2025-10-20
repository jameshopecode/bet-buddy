import { type FC } from 'react';
import { getMatch, useMatches } from 'src/hooks/useMatches.ts';
import { cn } from 'src/utils/cn.ts';
import { format } from 'date-fns';
import MarketRow from 'src/sections/markets/components/MarketRow.tsx';

interface IMarketsSectionProps {
  id: number
}

const MarketsSection: FC<IMarketsSectionProps> = ({ id }) => {
  const { data: match } = useMatches({ select: getMatch(id) });

  if (!match) {
    return null;
  }

  return <div className="flex flex-col gap-2">
    <div
      className={cn(
        "grid grid-cols-[100px_1fr_100px] gap-4",
        "rounded-md py-1 overflow-hidden mb-2"
      )}
    >
      <div className="flex flex-nowrap gap-4">
        <div className="font-bold whitespace-nowrap">{match.game}</div>
        <div className="text-gray-400 text-sm self-center whitespace-nowrap">{match.competition}</div>
      </div>
      <div className="inline-flex gap-2 justify-center font-bold">
        <span>{match.teams.home}</span>
        <span className="text-gray-400">vs</span>
        <span>{match.teams.away}</span>
      </div>
      <div className="text-sm text-gray-400 self-center whitespace-nowrap justify-self-end">
        {format(match.startTime, "yyyy-MM-dd HH:mm")}
      </div>
    </div>
    {Object.values(match.markets).map(market => (
      <MarketRow key={market.id} market={market} />
    ))}
  </div>;
};

export default MarketsSection;