import type { PropsWithChildren } from "react";

export function BlogPost(props: PropsWithChildren) {
  return <div className="blogPost">{props.children}</div>;
}
