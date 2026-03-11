import { createSemaphoreHook } from "./util.ts";

export const [useNavigationIsHidden, useHideNavigation] = createSemaphoreHook();
