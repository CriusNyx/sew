# Sew

See [this blog post](https://criusnyx.github.io/sew/#/blog) For a breakdown of
how to use sew.

Sew is an interactive command line tool that lets you write small string
manipulation programs and see your results in real time. Like any good CLI tool
it also supports input files and piped input.

I created sew because I wanted a tool that replaced the python scripts I was
using the manipulate strings in my clipboard. I wanted to create something that
had a nice grammar, was interactive in the command line, and had a user
experience with hotkeys, similar to nano.

# Building

For most systems the sew cli tool can be compiled by running.

```sh
cd cli
dotnet publish -c Release
```

If you want to compile sew as a single self executing file run.

```sh
cd cli
dotnet publish -c Release /p:PublishSingleFile=true /p:SelfContained=true
```

The output program will then be in the corresponding bin folder for your system.

Note that for single file publishing the binary for your system is located in
the `publish` sub directory inside of the build folder. The application inside
the build folder is not linked into a single file.

# Packages

## Core

This package contains the core logic for the sew runtime.

## CLI

This package contains the cli interface for using sew from the command line.

## JSI

This package contains bindings for wasm module generation.

## Site

This package contains the sew information website.

## Tests

This package contains unit tests.
