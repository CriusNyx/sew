import {
  createHashHistory,
  createBrowserHistory,
  createRouter,
  RouterProvider,
} from "@tanstack/react-router";
import { routeTree } from "../routeTree.gen.ts";

function createAppHistory() {
  switch (import.meta.env.VITE_PLATFORM) {
    case "github":
      return createHashHistory();
    default:
      return createBrowserHistory();
  }
}

const router = createRouter({ routeTree, history: createAppHistory() });

declare module "@tanstack/react-router" {
  interface Register {
    router: typeof router;
  }
}

export function AppRouter() {
  return <RouterProvider router={router} />;
}
