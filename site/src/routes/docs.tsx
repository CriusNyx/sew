import { createFileRoute } from "@tanstack/react-router";
import { useMemo } from "react";
import { SewJSI } from "../jsi/SewJSI.tsx";
import type {
  SewExample,
  SewMethod,
  SewOverload,
} from "../shemas/sewJSISchemas.ts";
import _ from "lodash";
import { interleaveComponent } from "../util/index.tsx";
import { SewExample as SewExampleComp } from "../components/SewExample.tsx";

export const Route = createFileRoute("/docs")({
  component: RouteComponent,
});

function RouteComponent() {
  const methods = useMemo(
    () => SewJSI.methods(),
    [],
  );

  const methodExamples = useMemo(
    () => _.groupBy(SewJSI.methodExamples(), (x) => x.MethodName),
    [],
  );

  return (
    <div className="flex flex-col">
      <h1>Docs</h1>
      {methods.map((x, i) => (
        <SewMethodView
          key={`method-${i}`}
          method={x}
          examples={methodExamples[_.first(x.Names) ?? ""]}
        />
      ))}
    </div>
  );
}

interface SewMethodViewProps {
  method: SewMethod;
  examples: SewExample[];
}

function SewMethodView({ method, examples }: SewMethodViewProps) {
  return (
    <div className="flex flex-col">
      <h4 className="text-accent mb-0!">{method.Names.join(", ")}</h4>
      <div className="h-0.5 w-full bg-accent my-2" />
      <div className="flex flex-col ml-5">
        {method.Overloads.map((x, i) => (
          <SewOverloadView
            key={`overload-${i}`}
            method={method}
            overload={x}
          />
        ))}
      </div>
      <div className="flex flex-col">
        {examples?.map((x, i) => (
          <SewExampleComp
            key={`example-${i}`}
            input={x.Input}
            program={x.Code}
          />
        ))}
      </div>
    </div>
  );
}

interface SewOverloadViewProps {
  method: SewMethod;
  overload: SewOverload;
}

function SewOverloadView({ method, overload }: SewOverloadViewProps) {
  return (
    <div className="flex flex-col">
      <div className="flex flex-col">
        <p>
          &#8226;&nbsp;
          <b>{_.first(method.Names)}</b>
          {overload.Args.length > 0 && (
            <>
              (
              {interleaveComponent(
                [...overload.Args, ...method.OptionalArgs].map((x, i) => (
                  <span key={`arg-${i}`}>
                    &ensp;<u>
                      {x.Name}
                    </u>:&nbsp;<span className="text-emerald-500/75">
                      {x.TypeName}

                      {x.DefaultValue && (
                        <>
                          &nbsp;=&nbsp;{x.DefaultValue}
                        </>
                      )}
                    </span>
                  </span>
                )),
                () => <span>,&nbsp;</span>,
              )}
              &ensp;)
            </>
          )}
        </p>
        <p className="pl-5">{overload.Description}</p>
      </div>
    </div>
  );
}
