import { createFileRoute } from "@tanstack/react-router";
import { SewInput } from "../components/SewInput.tsx";

export const Route = createFileRoute("/temp")({
  component: RouteComponent,
});

function RouteComponent() {
  return <SewInput />;
}
