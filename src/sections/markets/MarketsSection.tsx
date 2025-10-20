import { type FC } from 'react';
import { cn } from 'src/utils/cn.ts';
import { format } from 'date-fns';
import MarketRow from 'src/sections/markets/components/MarketRow.tsx';
import type { IMatch } from 'src/model/match.model.ts';

interface IMarketsSectionProps {
  className?: string
  match?: IMatch
}

const MarketsSection: FC<IMarketsSectionProps> = ({ className, match }) => {
  if (!match) {
    return null;
  }

  return (
    <div className={cn('flex flex-col gap-2', className)}>
      <div
        className={cn(
          'grid grid-cols-[100px_1fr_100px] gap-4',
          'mb-2 overflow-hidden rounded-md py-1'
        )}
      >
        <div className="flex flex-nowrap gap-4">
          <div className="font-bold whitespace-nowrap">{match.game}</div>
          <div className="self-center text-sm whitespace-nowrap text-gray-400">
            {match.competition}
          </div>
        </div>
        <div className="inline-flex justify-center gap-2 font-bold">
          <span>{match.home}</span>
          <span className="text-gray-400">vs</span>
          <span>{match.away}</span>
        </div>
        <div className="self-center justify-self-end text-sm whitespace-nowrap text-gray-400">
          {format(match.startTime, 'yyyy-MM-dd HH:mm')}
        </div>
      </div>
      {Object.values(match.markets).map(market => (
        <MarketRow key={market.id} market={market} />
      ))}
    </div>
  );
};

export default MarketsSection;