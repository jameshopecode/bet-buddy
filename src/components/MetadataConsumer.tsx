import type { FC } from 'react';
import {
  type ChatMessageWithMetadataT,
  getMatchesWithMarketsUrl,
} from 'src/model/chat.model.ts';
import { FaArrowRight } from 'react-icons/fa';

type Props = {
  message: ChatMessageWithMetadataT;
};

const MetadataConsumer: FC<Props> = ({ message }) => {
  const { metadata } = message;

  if (!metadata) {
    return null;
  }

  const url = getMatchesWithMarketsUrl(metadata);

  return (
    <div className="bg-dark w-4/5 rounded-sm p-3 text-white">
      <a
        className="flex w-fit items-center gap-2 rounded-md bg-gray-800 px-6 py-2"
        href={url}
        rel="noreferrer"
      >
        Browse markets
        <FaArrowRight />
      </a>
    </div>
  );
};

export default MetadataConsumer;
