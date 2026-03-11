import { createSemaphoreHook } from "./util.ts";

export const [useScrollIsLocked, useLockScroll] = createSemaphoreHook();
