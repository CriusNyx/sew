import { useEffect, useRef } from "react";

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function useRefCallback<const T extends any[], U>(
  func: (...args: T) => U,
): (...args: T) => U {
  const ref = useRef<((...args: T) => U)>(func);

  useEffect(() => {
    ref.current = func;
  }, [func]);

  // eslint-disable-next-line react-hooks/refs
  return ref.current;
}
