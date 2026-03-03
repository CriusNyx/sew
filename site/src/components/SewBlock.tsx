import { useState } from "react";
import { useAsyncMemo } from "use-async-memo";
import { sewJSI } from "sewjs/main.ts";
import { Copy } from "react-feather";
import { SewColored, SewInput } from "./SewInput.tsx";

export interface SewBlockProps {
  input?: string;
  program?: string;
  static?: boolean;
  editableInput?: boolean;
  fullSize?: boolean;
}

interface SewTextViewProps {
  text: string[];
  copyButton?: boolean;
}

export function SewTextContainer(props: SewTextViewProps) {
  return (
    <div className="flex flex-1 fira-mono relative grow shrink overflow-hidden text-sm fira-mono bg-zinc-900 rounded-2xl p-4">
      <div className="w-full overflow-scroll">
        <SewTextView text={props.text} />
      </div>
      {props.copyButton && (
        <button className="absolute top-2 right-2 bg-transparent!">
          <Copy />
        </button>
      )}
    </div>
  );
}

export function SewTextView(props: { text: string[] }) {
  if (props.text.length === 1) {
    return <pre className="">{props.text[0]}</pre>;
  }
  return (
    <div className="flex flex-col p-2 gap-2">
      {props.text.map((x) => (
        <pre className="p-2 rounded-md bg-zinc-950 min-h-[2em]">{x}</pre>
      ))}
    </div>
  );
}

interface SewTextInputProps {
  text: string;
  onChange: (value: string) => void;
}

export function SewTextInput(props: SewTextInputProps) {
  return (
    <textarea
      className="text-sm bg-zinc-900 fira-mono flex-1 p-4 rounded-xl overflow-scroll h-full w-full whitespace-pre"
      value={props.text}
      onChange={(e) => props.onChange(e.target.value)}
      placeholder="paste your input here"
    />
  );
}

export function SewBlock(props: SewBlockProps) {
  const [sewProgram, setSewProgram] = useState(props.program ?? "");
  const [input, setInput] = useState(props.input ?? "");

  const result = useAsyncMemo(async () => {
    return sewJSI.Process(input ?? "", sewProgram);
  }, [input, sewProgram]);

  return (
    <div
      className={`flex flex-col gap-4 pb-4 items-stretch ${
        props.fullSize ? "h-full shrink" : ""
      }`}
    >
      <div
        className={`flex shrink relative ${
          props.fullSize ? "flex-1 max-h-full" : "h-100"
        }`}
      >
        <div
          className={`absolute top-0 bottom-0 left-0 right-0 flex lg:flex-row gap-4`}
        >
          {(props.editableInput && (
            <SewTextInput text={input} onChange={setInput} />
          )) || <SewTextContainer text={[input]} />}
          <SewTextContainer
            copyButton
            text={
              (result && (result.hasValue ? result.value : [result.error])) ||
              []
            }
          />
        </div>
      </div>
      <div className="flex shrink flex-row items-center p-2 px-4 bg-zinc-900 rounded-xl">
        {!props.static && (
          <>
            <span>Program:&nbsp;&nbsp;</span>

            <SewInput
              className="flex flex-1 outline-0 bg-zinc-900 fira-mono"
              placeholder="Sew program source code"
              value={sewProgram}
              onChange={(value) => setSewProgram(value)}
            />
          </>
        )}
        {props.static && (
          <>
            <span>Program:&nbsp;&nbsp;</span>
            <SewColored text={props.program ?? ""} />
          </>
        )}
      </div>
    </div>
  );
}
