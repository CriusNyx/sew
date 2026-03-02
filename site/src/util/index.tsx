import type React from "react";

export function interleaveComponent(
  elements: React.ReactNode[],
  Interleave: React.ComponentType,
) {
  let interleaveIndex = 0;
  return elements.reduce(
    (prev: React.ReactNode[], curr) =>
      prev.length === 0
        ? [curr]
        : [
            ...prev,
            <Interleave key={`interleave-${interleaveIndex++}`} />,
            curr,
          ],
    [],
  );
}
