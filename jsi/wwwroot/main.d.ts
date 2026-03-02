export type JSIResult = { hasValue: true; value: string } | {
  hasValue: false;
  error: string;
};
