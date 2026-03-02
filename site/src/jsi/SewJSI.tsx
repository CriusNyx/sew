import { sewJSI } from "sewjs/main.ts";
import { SewSchema } from "../shemas/sewJSISchemas.ts";

function methods() {
  const methods = sewJSI.Methods();
  return SewSchema.SewMethod.array().parse(methods);
}

function methodExamples() {
  const examples = sewJSI.MethodExamples();
  return SewSchema.SewExample.array().parse(examples);
}

export const SewJSI = {
  methods,
  methodExamples,
  analyzeSemantics: sewJSI.AnalyzeSemantics,
};
