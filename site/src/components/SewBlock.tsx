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

export function SewTextView(props: SewTextViewProps) {
  return (
    <div className="flex flex-1 fira-mono relative grow shrink overflow-hidden">
      <pre className="text-sm bg-zinc-900 flex-1 p-4 rounded-xl overflow-scroll whitespace-pre">
        {props.text.join("\n")}
      </pre>
      {props.copyButton && (
        <button className="absolute top-2 right-2 bg-transparent!">
          <Copy />
        </button>
      )}
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
      className={`flex flex-col gap-2 pb-4 items-stretch ${
        props.fullSize ? "h-full" : ""
      }`}
    >
      <div
        className={`flex flex-col lg:flex-row gap-2 shrink ${
          props.fullSize ? "flex-1" : "max-h-100"
        }`}
      >
        {(props.editableInput && (
          <SewTextInput text={input} onChange={setInput} />
        )) || <SewTextView text={[input]} />}
        <SewTextView
          copyButton
          text={
            (result && (result.hasValue ? result.value : [result.error])) || []
          }
        />
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
