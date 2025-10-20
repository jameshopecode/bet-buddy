import {
  type ChangeEvent,
  type ComponentProps, type FC,
  useCallback,
  useState,
} from 'react';
import { cn } from 'src/utils/cn.ts';

interface Props extends Omit<ComponentProps<'textarea'>, "onChange"> {
  value: string
  onChange(value: string): void
}

const TextInput: FC<Props> = ({
  className,
  value,
  onChange,
  ...props
}) => {
  const handleChange = useCallback(
    (event: ChangeEvent<HTMLTextAreaElement>) => {
      onChange(event.target.value);
    },
    []
  );

  return (
    <div className="grid grid-cols-[1fr_auto] gap-4 items-center">
      <textarea
        data-slot="textarea"
        value={value}
        onChange={handleChange}
        className={cn(
          'border-input placeholder:text-muted-foreground focus-visible:border-ring focus-visible:ring-ring/50 aria-invalid:ring-destructive/20 dark:aria-invalid:ring-destructive/40 aria-invalid:border-destructive dark:bg-input/30 flex field-sizing-content min-h-16 w-full rounded-md border bg-transparent px-3 py-2 text-base shadow-xs transition-[color,box-shadow] outline-none focus-visible:ring-[3px] disabled:cursor-not-allowed disabled:opacity-50 md:text-sm',
          className
        )}
        {...props}
      />
    </div>
  );
};

export default TextInput;
