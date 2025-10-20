import { useMutation } from '@tanstack/react-query';
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
        })
          .then(async response => {
            if (!response.ok) {
              throw new Error(`Request error with status ${response.status}: ${response.statusText}`)
            }

            const text = await response.text();
            try {
              return JSON.parse(text);
            } catch {
              return {
                answer: text,
              }
            }
          })
    },
    queryClient
  );
};
