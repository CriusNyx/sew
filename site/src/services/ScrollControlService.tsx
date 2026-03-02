import {
  createContext,
  type PropsWithChildren,
  useContext,
  useEffect,
  useState,
} from "react";
import _ from "lodash";
import { useRefCallback } from "../util/useRefCallback.ts";

interface ScrollControlContextState {
  registerLockScroll(): number;
  freeLockScroll(value: number): void;
  isLocked: boolean;
}

const ScrollControlContext = createContext<ScrollControlContextState>({
  registerLockScroll() {
    return -1;
  },
  freeLockScroll() {},
  isLocked: false,
});

export function ScrollControlService(props: PropsWithChildren) {
  const [scrollLocks, setScrollLocks] = useState<number[]>([]);

  const registerLockScroll = useRefCallback(
    function registerLockScroll() {
      const next = (_.max(scrollLocks) ?? -1) + 1;
      setScrollLocks((prev) => [...prev, next]);
      return next;
    },
  );

  const freeLockScroll = useRefCallback(
    function freeLockScroll(value: number) {
      setScrollLocks((prev) => prev.filter((x) => x !== value));
    },
  );

  return (
    <ScrollControlContext.Provider
      value={{
        registerLockScroll,
        freeLockScroll,
        isLocked: scrollLocks.length !== 0,
      }}
    >
      {props.children}
    </ScrollControlContext.Provider>
  );
}

export function useScrollIsLocked() {
  const context = useContext(ScrollControlContext);
  return context.isLocked;
}

export function useLockScroll(locked?: boolean) {
  let isLocked = locked;
  if (isLocked === null || isLocked === undefined) {
    isLocked = true;
  }

  const context = useContext(ScrollControlContext);
  useEffect(() => {
    if (isLocked) {
      const key = context.registerLockScroll();
      return () => {
        context.freeLockScroll(key);
      };
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isLocked]);
}
