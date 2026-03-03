import { useState } from "react";
import { ChevronDown, ChevronRight } from "react-feather";
import { SewBlock } from "./SewBlock.tsx";
import { SewColored } from "./SewInput.tsx";

export interface SewExampleProps {
  input?: string;
  program?: string;
}

export function SewExample(props: SewExampleProps) {
  const [open, setOpen] = useState(false);
  return (
    <>
      <div className={`flex flex-col bg-zinc-900 mb-4`}>
        <span
          className="flex flex-row items-center p-2 cursor-pointer"
          onClick={() => setOpen(!open)}
        >
          {open ? <ChevronDown size={12} /> : <ChevronRight size={12} />}
          <SewColored className="pl-2" text={props.program ?? ""} />
        </span>
        {open && (
          <span className="p-4 pb-0 bg-zinc-800">
            <SewBlock {...props} static />
          </span>
        )}
      </div>
    </>
  );
}

export function SewSnippet(props: { text: string }) {
  return <SewColored className="p-2 bg-zinc-900" text={props.text} />;
}
