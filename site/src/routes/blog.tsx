import { createFileRoute } from "@tanstack/react-router";
import Blog from "./-blog.mdx";
import "./blog.css";

export const Route = createFileRoute("/blog")({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <div className="sew">
      <Blog />
    </div>
  );
}
