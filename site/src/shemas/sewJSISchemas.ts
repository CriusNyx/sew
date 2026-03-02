import z from "zod";

function optionalOrNull<T extends z.ZodType>(zodSchema: T) {
  return zodSchema.optional().or(z.null().transform(() => undefined));
}

const SewArg = z.object({
  Name: z.string(),
  TypeName: z.string(),
  DefaultValue: optionalOrNull(z.string()),
  Description: optionalOrNull(z.string()),
});

const SewOverload = z.object({
  Description: z.string(),
  Args: z.array(SewArg),
});

const SewMethod = z.object({
  Names: z.array(z.string()),
  Returns: z.string(),
  Overloads: z.array(SewOverload),
  OptionalArgs: z.array(SewArg),
});

const SewExample = z.object({
  MethodName: z.string(),
  Input: z.string(),
  Code: z.string(),
});

export type SewMethod = z.output<typeof SewMethod>;
export type SewOverload = z.output<typeof SewOverload>;
export type SewArg = z.output<typeof SewArg>;
export type SewExample = z.output<typeof SewExample>;

export const SewSchema = {
  SewArg,
  SewOverload,
  SewMethod,
  SewExample,
};
