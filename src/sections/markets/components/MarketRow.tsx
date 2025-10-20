import { type FC } from 'react';
import type { IMarket } from 'src/model/market.model.ts';
import { cn } from 'src/utils/cn.ts';

interface IMarketRowProps {
  market: IMarket
}

const MarketRow: FC<IMarketRowProps> = ({ market }) => {
  const [selection1, selection2] = market.selections ?? [];
  return (
    <div
      className={cn(
        "grid grid-cols-[1fr_1fr] gap-x-4 gap-y-1 [grid-template-areas:'name_name''selection1_selection2']",
        "bg-gray w-full rounded-md p-2 overflow-hidden"
      )}
    >
      <div className="[grid-area:name] font-bold text-center text-gray-300">{market.name}</div>
      <div className="[grid-area:selection1] grid grid-cols-[1fr_auto] items-center gap-2">
        <div className="text-end font-bold">{selection1.name}</div>
        <button className="h-8 w-14 rounded-sm transition-colors bg-gray-100/10 hover:bg-gray-100/30 cursor-pointer">
          {selection1.odds}
        </button>
      </div>
      <div className="[grid-area:selection2] grid grid-cols-[auto_1fr] items-center gap-2">
        <button className="h-8 w-14 rounded-sm transition-colors bg-gray-100/10 hover:bg-gray-100/30 cursor-pointer">
          {selection2.odds}
        </button>
        <div className=" font-bold">{selection2.name}</div>
      </div>
    </div>
  );
};

export default MarketRow;