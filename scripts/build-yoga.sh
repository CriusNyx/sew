#!/bin/bash

SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )

(
  cd "$SCRIPT_DIR/../Yoga.NET";

  echo "Building Yoga";

  cmake -DCMAKE_BUILD_TYPE=Release -S . -B build;
  cmake --build build;

  dotnet build;
)