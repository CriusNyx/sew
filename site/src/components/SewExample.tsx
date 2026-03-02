import { useState } from "react";
import { ChevronDown, ChevronRight } from "react-feather";
import { SewBlock } from "./SewBlock.tsx";

export interface SewExampleProps {
  input?: string;
  program?: string;
}

export function SewExample(props: SewExampleProps) {
  const [open, setOpen] = useState(false);
  return (
    <>
      <div
        className={`flex flex-col pointer bg-zinc-900 mb-4`}
      >
        <span
          className="flex flex-row items-center p-2"
          onClick={() => setOpen(!open)}
        >
          {open ? <ChevronDown size={12} /> : <ChevronRight size={12} />}
          <pre>&nbsp;{props.program}</pre>
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
