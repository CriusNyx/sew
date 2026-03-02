import type { AppService } from "./types.ts";
import type { PropsWithChildren } from "react";

interface ServiceProviderProps extends PropsWithChildren {
  services: AppService[];
}

export function ServiceProvider(props: ServiceProviderProps) {
  return props.services.reduceRight(
    (prev, Service) => <Service>{prev}</Service>,
    props.children,
  );
}
