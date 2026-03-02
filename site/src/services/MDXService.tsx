import type { PropsWithChildren } from "react";
import { MDXProvider } from "@mdx-js/react";
import { SewBlock } from "../components/SewBlock.tsx";
import { SewExample } from "../components/SewExample.tsx";
import { Link } from "@tanstack/react-router";

function CustomH3(props: PropsWithChildren) {
  return (
    <>
      <h3 className="mb-0!">{props.children}</h3>
      <div className="h-0.5 bg-slate-700 w-full mb-4" />
    </>
  );
}

export function MDXService(props: PropsWithChildren) {
  return (
    <MDXProvider
      components={{
        SewBlock,
        SewExample,
        Link,
        h3: CustomH3,
      }}
    >
      {props.children}
    </MDXProvider>
  );
}
