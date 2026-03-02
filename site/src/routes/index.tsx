import { createFileRoute, Link } from "@tanstack/react-router";
import { SewBlock } from "../components/SewBlock.tsx";
import { useLockScroll } from "../services/ScrollControlService.tsx";

export const Route = createFileRoute("/")({
  component: RouteComponent,
});

function RouteComponent() {
  useLockScroll();

  return (
    <div className="flex flex-col h-full gap-5">
      <div className="flex flex-row gap-5 justify-center">
        <div className="flex flex-row gap-5 items-center p-2 px-5 bg-zinc-900 rounded-2xl">
          <a href={"https://github.com/CriusNyx/sew/"}>Install Sew</a>
          &#x25cf;
          <Link to={"/blog"}>
            Introduction to Sew
          </Link>
          &#x25cf;
          <Link to={"/docs"}>Docs</Link>
        </div>
      </div>
      <SewBlock editableInput fullSize />
    </div>
  );
}
