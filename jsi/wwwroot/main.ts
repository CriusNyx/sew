// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// @ts-types="./stubs/dotnet.d.ts"
import { dotnet } from "./_framework/dotnet.js";

const { setModuleImports, getAssemblyExports, getConfig, runMain } =
  await dotnet
    .withApplicationArguments("start")
    .create();

export type JSIResult = {
  hasValue: true;
  value: string[];
} | { hasValue: false; error: string };

const semanticTypes = [
  "none",
  "keyword",
  "method",
  "identifier",
  "numLit",
  "regexLit",
  "stringLit",
] as const;

export type SemanticType = typeof semanticTypes[number];

export interface SemanticToken {
  Start: number;
  Length: number;
  SemanticType: SemanticType;
}

const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);

export const sewJSI = {
  Process(input: string, sewProgram: string): JSIResult {
    const result = exports.SewJSI.Process(input, sewProgram);
    return JSON.parse(result);
  },
  Methods(): any[] {
    const result = exports.SewJSI.Methods();
    return JSON.parse(result);
  },
  MethodExamples(): any[] {
    const result = exports.SewJSI.MethodExamples();
    return JSON.parse(result);
  },
  AnalyzeSemantics(program: string): SemanticToken[] {
    return JSON.parse(
      exports.SewJSI.AnalyzeSemantics(program),
    ) as SemanticToken[];
  },
};

// run the C# Main() method and keep the runtime process running and executing further API calls
await runMain();
