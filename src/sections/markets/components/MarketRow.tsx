import { type FC } from 'react';
import type { IMarket } from 'src/model/market.model.ts';
import { cn } from 'src/utils/cn.ts';

interface IMarketRowProps {
  market: IMarket
}

const isDraw = (selection: IMarket["selections"][number]) => {
  return ["draw", "neither"].includes(selection?.name);
}

const getSelections = (selections: IMarket["selections"]) => {
  const home = selections.find(s => !isDraw(s))!;
  const draw = selections.find(s => isDraw(s));
  const away = selections.findLast(s => !isDraw(s))!;

  return { home, draw, away }
}

const formatOdds = (value: number) => value.toFixed(2);

const MarketRow: FC<IMarketRowProps> = ({ market }) => {
  const { home, draw, away } = getSelections(market.selections ?? []);

  return (
    <div
      className={cn(
        "grid grid-cols-[1fr_56px_1fr] gap-x-2 gap-y-1 [grid-template-areas:'name_name_name''home_draw_away']",
        "bg-gray w-full rounded-md p-2 overflow-hidden"
      )}
    >
      <div className="[grid-area:name] font-bold text-center text-gray-300">{market.name}</div>
      <div className="[grid-area:home] grid grid-cols-[1fr_auto] items-center gap-2">
        <div className="text-end font-bold">{home.name}</div>
        <button className="h-8 w-14 rounded-sm transition-colors bg-gray-950/100 hover:bg-gray-950/40 cursor-pointer">
          {formatOdds(home.odds)}
        </button>
      </div>
      {!!draw &&
        <div className="[grid-area:draw]">
          <button className="h-8 w-14 rounded-sm transition-colors bg-gray-950/100 hover:bg-gray-950/40 cursor-pointer">
            {formatOdds(draw.odds)}
          </button>
        </div>
      }
      <div className="[grid-area:away] grid grid-cols-[auto_1fr] items-center gap-2">
        <button className="h-8 w-14 rounded-sm transition-colors bg-gray-950/100 hover:bg-gray-950/40 cursor-pointer">
          {formatOdds(away.odds)}
        </button>
        <div className=" font-bold">{away.name}</div>
      </div>
    </div>
  );
};

export default MarketRow;