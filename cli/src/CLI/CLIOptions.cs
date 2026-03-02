using CommandLine;
using CriusNyx.Results;

namespace Sew.CLI;

public class CLIOptions
{
  [Value(
    0,
    Default = null,
    HelpText = "Sew program to run",
    Required = false,
    MetaName = "program"
  )]
  public string? Program { get; set; }

  [Option('i', "input-file", Required = false, HelpText = "Input file to read from")]
  public string? InputFile { get; set; }

  [Option(
    'o',
    "output-file",
    Required = false,
    HelpText = "Output file to write to. If unspecified will write to the command line instead."
  )]
  public string? OutputFile { get; set; }

  [Option(
    'c',
    "clipboard",
    Required = false,
    Default = false,
    HelpText = "Get the input from the clipboard. If no output is specified write the output back to the clipboard."
  )]
  public bool Clipboard { get; set; }

  [Option("docs", Required = false, Default = false, HelpText = "Print the docs and exit.")]
  public bool Docs { get; set; }

  public bool Interactive => Program == null;

  public static Result<CLIOptions, IEnumerable<Error>> Parse(string[] args)
  {
    var parsed = Parser.Default.ParseArguments<CLIOptions>(args);

    if (parsed.Value != null)
    {
      return parsed.Value;
    }
    return Result.Err<CLIOptions, IEnumerable<Error>>(parsed.Errors);
  }
}
