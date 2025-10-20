import { type Context, useContext } from 'react';

export const getSafeContext = <T>(
  context: Context<T | null>,
  name = 'context'
) => {
  return () => {
    const ctx = useContext(context);

    if (!ctx) {
      throw new UnreachableError(name, `Missing ${name} data!`);
    }
    return ctx;
  };
};

export class UnreachableError extends Error {
  constructor(_nvr: never | string, message: string) {
    super(message);
  }
}
