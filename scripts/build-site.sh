#!/bin/bash

SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )

BASE=$1

(
  cd $SCRIPT_DIR/../site;
  deno run build:$BASE;
)