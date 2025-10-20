import {
  type ChangeEvent,
  type ComponentProps,
  type FC,
  useCallback,
  useState,
} from 'react';
import { cn } from 'src/utils/cn.ts';
import { IoSendOutline } from 'react-icons/io5';
import { AiOutlineLoading } from 'react-icons/ai';

//@ts-expect-error nie krzycz na mnie poniedziałek jest
interface Props extends Omit<ComponentProps<'textarea'>, 'onChange'> {
  value: string;
  onChange(value: string): void;
  onSubmit?(value: string): void;
  isPending?: boolean
}

const TextInput: FC<Props> = ({
  className,
  value,
  onChange,
  onSubmit,
  isPending,
  ...props
}) => {
  const disabled = isPending || !value?.trim();
  const checkForEnter = (event: KeyboardEvent) => {
    if (event.key === 'Enter' && !disabled) {
      event.preventDefault();
      onSubmit?.(value);
    }
  };

  const handleChange = useCallback(
    (event: ChangeEvent<HTMLTextAreaElement>) => {
      onChange(event.target.value);
    },
    []
  );

  const handleButtonClick = () => {
    if (disabled) {
      return;
    }
    onSubmit?.(value);
  }

  return (
    <div className="grid grid-cols-[1fr_auto] items-center gap-4 p-1">
      <textarea
        data-slot="textarea"
        value={value}
        //@ts-expect-error nie krzycz na mnie poniedziałek jest
        onKeyDown={checkForEnter}
        onChange={handleChange}
        className={cn(
          'border border-gray-500 focus-visible:border-ring focus-visible:ring-gray-400 placeholder:text-muted-foreground',
          'bg-gray-950/30 flex field-sizing-content min-h-16 w-full rounded-md px-3 py-2',
          'text-base shadow-xs transition-[color,box-shadow] outline-none',
          'focus-visible:ring-[3px] disabled:cursor-not-allowed disabled:opacity-50 md:text-sm',
          className
        )}
        {...props}
      />
      <button
        className="p-2 rounded-full cursor-pointer transition-colors bg-gray-100/10 hover:bg-gray-100/30"
        onClick={handleButtonClick}
        disabled={disabled}
      >
        {isPending ?
          <AiOutlineLoading className="animate-spin" />
          :
          <IoSendOutline />
        }
      </button>
    </div>
  );
};

export default TextInput;
