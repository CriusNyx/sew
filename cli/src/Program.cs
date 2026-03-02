// This comment is here because there is a bug in VSCode where it is folding line 1 unless it is a comment.
using CriusNyx.Results;
using CriusNyx.Util;
using Sew;
using Sew.CLI;

void PrintLog()
{
  foreach (var (severity, message) in Logger.Instance.Messages)
  {
    PrintMessage(severity, message);
  }
}

void PrintMessage(LogSeverity severity, object o)
{
  switch (severity)
  {
    case LogSeverity.Log:
      Console.WriteLine(o);
      break;
#if DEBUG
    case LogSeverity.Debug:
      Console.WriteLine(o);
      break;
#endif
    case LogSeverity.Error:
      Console.Error.WriteLine(o);
      break;
  }
}

Result<string, AppException> ReadInputText(CLIOptions opts)
{
  if (opts.InputFile != null)
  {
    return File.ReadAllText(opts.InputFile);
  }
  if (opts.Clipboard && TextCopy.ClipboardService.GetText() is string clipboard)
  {
    return Clipboard.GetClipboard();
  }
  if (Console.IsInputRedirected)
  {
    if (opts.Interactive)
    {
      return new AppException(
        new CommonError(
          "InputRedirectError",
          "Cannot start in interactive mode when input is redirected."
        )
      );
    }
    return Function.UntilNull(Console.ReadLine).StringJoin("\n");
  }
  return new AppException(new CommonError("NoInput", "No input source was provided"));
}

void StartApp(CLIOptions opts)
{
  if (opts.Docs)
  {
    Console.WriteLine(SewRuntime.Methods.Select(x => x.ToString()).StringJoin("\n\n"));
    return;
  }

  var inputResult = ReadInputText(opts);

  if (inputResult.IsErr())
  {
    var error = inputResult.Err().Unwrap();
    Console.WriteLine(error.FormatError(true));
    return;
  }

  var input = inputResult.Unwrap();

  if (opts.Interactive)
  {
    StartRepl(opts, new() { Input = input });
    return;
  }

  var sewInput = opts.Program ?? "";
  var eval = SewLang.Eval(input, sewInput).Unwrap().Output;
  Console.Write(eval.StringJoin("\n"));
}

void StartRepl(CLIOptions opts, ReplConfig config)
{
  new Repl(config)
    .Start()
    .Inspect(
      (result) =>
      {
        var eval = SewLang.Eval(result.State.textInput, result.State.sewInput);

        if (eval.IsErr())
        {
          return;
        }

        var output = eval.Unwrap().Output.StringJoin("\n");

        switch (result.Type)
        {
          case ReplOutputType.outputToFile:
            if (opts.OutputFile is string outputFile)
            {
              File.WriteAllText(outputFile, output);
            }
            break;
          case ReplOutputType.outputToClipboard:
            Clipboard.Paste(output);
            break;
          case ReplOutputType.quit:
            Console.WriteLine(output);
            break;
        }
      }
    );
}

void Main()
{
  try
  {
    CLIOptions
      .Parse(args)
      .Inspect(StartApp)
      .InspectErr(
        (errors) =>
        {
          if (errors.Any(x => !(x is CommandLine.HelpRequestedError)))
          {
            foreach (var error in errors)
            {
              Console.Error.WriteLine(error);
            }
          }
        }
      );

    PrintLog();
  }
  catch (Exception e)
  {
    PrintLog();
    Console.Error.WriteLine(e);
  }
}

Main();
