import { useQuery } from '@tanstack/react-query';
import { queryClient } from 'src/core/query/query-client.ts';

interface IUseMatchesInput {
  pageSize: number
}

export function useMatches({ pageSize = 10 }: IUseMatchesInput) {
  const { data } = useQuery({
    queryKey: ["matches", pageSize],
    queryFn: async () => {
      console.log("useMatches")
      return await fetch("https://jsonplaceholder.typicode.com/posts")
        .then(response => response.json());
    },
  }, queryClient);

  return { matches: data };
}