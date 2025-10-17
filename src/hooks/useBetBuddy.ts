import { useMutation, useQuery } from '@tanstack/react-query';
import { queryClient } from 'src/core/query/query-client.ts';

export const useBetBuddy = () => {
  return useMutation(
    {
      mutationFn: async ({
        question,
        userId,
      }: {
        question: string;
        userId: string;
      }) =>
        await fetch('/api/buddy/chat', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            question,
            userId,
          }),
        }),
    },
    queryClient
  );
};
