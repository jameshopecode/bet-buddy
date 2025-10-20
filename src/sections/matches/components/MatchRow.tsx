import type { FC } from 'react';
import type { IMatch } from 'src/model/match.model.ts';
import { FaAngleRight } from 'react-icons/fa6';
import { cn } from 'src/utils/cn.ts';
import { format } from 'date-fns';

interface IMatchRowProps {
  match: IMatch;
}

const MatchRow: FC<IMatchRowProps> = ({ match }) => {
  return (
    <div
      className={cn(
        "grid grid-cols-[70px_1fr_70px] [grid-template-areas:'game_time_button''competition_teams_button']",
        "bg-gray w-full rounded-md p-2 overflow-hidden"
      )}
    >
      <div className="[grid-area:game] ml-2 font-bold">{match.game}</div>
      <div className="[grid-area:competition] ml-2 text-gray-400">{match.competition}</div>
      <div className="inline-flex gap-2 justify-center [grid-area:time] text-sm text-gray-400">
        {format(match.startTime, "yyyy-MM-dd HH:mm")}
      </div>
      <div className="inline-flex gap-2 justify-center [grid-area:teams] font-bold">
        <span>{match.home}</span>
        <span className="text-gray-400">vs</span>
        <span>{match.away}</span>
      </div>
      <a href={match.url} className="justify-self-end flex items-center flex-row gap-2 font-bold transition-colors bg-gray-100/0 hover:bg-gray-100/10 -m-2 p-4 [grid-area:button]">
        <div className="text-md/14px -mb-[2px]">{Object.keys(match.markets).length}</div>
        <FaAngleRight />
      </a>
    </div>
  );
};

export default MatchRow;
