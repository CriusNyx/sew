import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/install")({
  component: RouteComponent,
});

function RouteComponent() {
  return <div>¯\_(ツ)_/¯</div>;
}
