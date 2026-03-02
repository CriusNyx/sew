import { useState } from "react";
import "./App.css";
import { AppRouter } from "./routes/-router.tsx";
import { ServiceProvider } from "./services/index.tsx";
import { MDXService } from "./services/MDXService.tsx";
import type { AppService } from "./services/types.ts";
import { ScrollControlService } from "./services/ScrollControlService.tsx";

function App() {
  const [services] = useState<AppService[]>([ScrollControlService, MDXService]);

  return (
    <>
      <ServiceProvider services={services}>
        <AppRouter />
      </ServiceProvider>
    </>
  );
}

export default App;
