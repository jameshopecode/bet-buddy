import { useMutation } from '@tanstack/react-query';
import { queryClient } from 'src/core/query/query-client.ts';

export function useCleanChatHistory({ onSuccess }: { onSuccess?(): void }) {
  const { mutateAsync, isPending } = useMutation({
    mutationFn: async () => {
      await fetch('/api/data/cleanup', {
        method: 'POST',
      });
    },
    onSuccess,
  }, queryClient)

  return { cleanChatHistory: mutateAsync, isPending }
}