import { useEffect, useMemo, useState, type ReactElement } from "react";
import type { SemanticToken } from "sewjs/main.ts";
import { SewJSI } from "../jsi/SewJSI.tsx";

export function GenerateSewSpans(
  program: string,
  tokens: SemanticToken[],
): ReactElement[] {
  return tokens.map((x, i) => (
    <span
      className={`whitespace-pre flex-nowrap text-semantic-${x.SemanticType}`}
      key={`token-${i}`}
    >
      {program.substring(x.Start, x.Start + x.Length)}
    </span>
  ));
}

interface SewInputProps {
  className?: string;
  placeholder?: string;
  value?: string;
  onChange?: (value: string) => void;
}

export function SewInput(props: SewInputProps) {
  const [value, setValue] = useState("");

  useEffect(() => {
    if (props.value && value !== props.value) {
      setValue(props.value);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props.value]);

  function onChange(value: string) {
    setValue(value);
    props.onChange?.(value);
  }

  return (
    <div className={`relative bg-zinc-900 ${props.className}`}>
      <input
        style={{ padding: 0, margin: 0 }}
        className="w-full fira-mono text-transparent caret-white"
        placeholder={props.placeholder}
        value={value}
        onChange={(e) => onChange(e.target.value)}
      />
      <SewColored
        style={{ padding: 0, margin: 0 }}
        className="w-full absolute top-0 left-0 select-none pointer-events-none inline-block whitespace-nowrap overflow-hidden"
        text={value}
      />
    </div>
  );
}

interface SewColoredProps {
  style?: React.CSSProperties;
  className?: string;
  text: string;
}

export function SewColored(props: SewColoredProps) {
  const analysis = useMemo(
    () => SewJSI.analyzeSemantics(props.text),
    [props.text],
  );

  if (analysis.length === 0) {
    return (
      <pre style={props.style} className={props.className}>
        {props.text}
      </pre>
    );
  }

  return (
    <pre style={props.style} className={props.className}>
      {GenerateSewSpans(props.text, analysis)}
    </pre>
  );
}
