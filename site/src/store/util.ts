import { atom as createAtom, useAtomValue, useSetAtom } from "jotai";
import { useEffect } from "react";

export function createSemaphoreHook() {
  const atom = createAtom(0);

  function useIsLocked() {
    return useAtomValue(atom) !== 0;
  }

  function useLockSemaphore() {
    const setAtom = useSetAtom(atom);
    useEffect(() => {
      setAtom((value) => value + 1);
      return () => setAtom((value) => value - 1);
      // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);
  }

  return [useIsLocked, useLockSemaphore] as const;
}
