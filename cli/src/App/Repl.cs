using System.Threading.Channels;
using CriusNyx.Results;
using CriusNyx.Util;
using Sharpie;
using Sharpie.Abstractions;
using Sharpie.Backend;

namespace Sew.CLI;

/// <summary>
/// Contains information for print hotkeys to the screen.
/// </summary>
/// <param name="key"></param>
/// <param name="descriptor"></param>
public struct Hotkey(string key, string descriptor)
{
  public void Draw(Repl app, IWindow window, ColorMixture hotkeyColor)
  {
    window.WriteText($" {key} ", new Style { ColorMixture = hotkeyColor });
    window.WriteText(" ");
    window.WriteText(descriptor);
    window.WriteText("   ");
  }
}

/// <summary>
/// Create Read Eval Print Loop for Sew
/// </summary>
public class Repl
{
  /// <summary>
  /// Global hotkeys that Sew supports.
  /// </summary>
  public Hotkey[] hotkeys { get; private set; } =
  [
    new("CTRL+A", "AST"),
    new("CTRL+C", "Exit"),
    new("CTRL+X", "Copy and Exit"),
    new("CTRL+D", "Toggle Debug"),
  ];

  /// <summary>
  /// The configuration used to initialize the repl.
  /// </summary>
  public readonly ReplConfig replConfig;

  /// <summary>
  /// Current state.
  /// </summary>
  public State state { get; private set; }

  /// <summary>
  /// Used to kill other threads.
  /// </summary>
  private CancellationTokenSource cancellationTokenSource;

  /// <summary>
  /// Channel used to aggregate events on the main thread.
  /// </summary>
  public Channel<ReplEvent> eventChannel;

  /// <summary>
  /// Memoized function for evaluating sew source code.
  /// </summary>
  private Func<string, Task<Result<SewLangOutput, SewLangException>>> ProcessSewInput;

  /// <summary>
  /// Latest output from the most recent sew input.
  /// </summary>
  private LatestResultChannel<string, Result<SewLangOutput, SewLangException>> LastSewOutput;

  /// <summary>
  /// Create a new repl.
  /// </summary>
  /// <param name="appConfig"></param>
  public Repl(ReplConfig appConfig)
  {
    this.replConfig = appConfig;
    state = State.From(appConfig);
    cancellationTokenSource = new CancellationTokenSource();
    eventChannel = null!;

    LastSewOutput = new LatestResultChannel<string, Result<SewLangOutput, SewLangException>>(
      (sewInput) =>
        Task.Run(() =>
        {
          return SewLang.Eval(state.textInput, sewInput);
        })
    );

    ProcessSewInput = Function.Memo<string, Task<Result<SewLangOutput, SewLangException>>>(
      LastSewOutput.Queue
    );
  }

  /// <summary>
  /// Start the repl and return the result.
  /// </summary>
  /// <returns></returns>
  public Result<ReplOutput, Exception> Start()
  {
    // Is only unreachable on embedded systems. We don't care about interactive CLI on embedded systems.
#pragma warning disable CA1416 // Validate platform compatibility
    using var terminal = new Terminal(
      CursesBackend.Load(),
      new(ManagedWindows: true, UseMouse: false, CaretMode: CaretMode.Invisible)
    );
#pragma warning restore CA1416 // Validate platform compatibility

    return StartRepl(terminal);
  }

  /// <summary>
  /// Create a new blocking event reader for the main thread.
  /// </summary>
  /// <param name="terminal"></param>
  /// <returns></returns>
  private IEnumerable<ReplEvent> CreateEventReader(Terminal terminal)
  {
    var eventListenerTask = Task.Run(
      async () =>
      {
        foreach (var @event in terminal.Events.Listen())
        {
          var appEvent = ReplEvent.FromSharpieEvent(@event);
          await eventChannel.Writer.WriteAsync(appEvent);
        }
      },
      cancellationTokenSource.Token
    );

    ReplEvent ReadOne()
    {
      return eventChannel.Reader.ReadAsync().AsTask().Touch(task => task.Wait()).Result;
    }

    IEnumerable<ReplEvent> Reader()
    {
      while (true)
      {
        yield return ReadOne();
      }
    }

    return Reader();
  }

  /// <summary>
  /// Create event pump to create ReplEvents when Sew is done evaluating a program.
  /// </summary>
  private void CreateSewOutputEventPump()
  {
    Task.Run(async () =>
    {
      while (true)
      {
        var result = await LastSewOutput.Reader.ReadAsync();
        var @event = ReplEvent.ProcessedTextEvent(result);
        eventChannel?.Writer.WriteAsync(@event);
      }
    });
  }

  /// <summary>
  /// Start the Repl and return the result.
  /// </summary>
  /// <param name="terminal"></param>
  /// <returns></returns>
  private Result<ReplOutput, Exception> StartRepl(Terminal terminal)
  {
    eventChannel = Channel.CreateUnbounded<ReplEvent>();

    var replInterface = ReplInterface.CreateAndInitialize(this, terminal);
    CreateSewOutputEventPump();

    foreach (var @event in CreateEventReader(terminal))
    {
      state.ProcessEvent(@event, null);

      ProcessSewInput(state.sewInput);

      if (state.currentAppEvent?.type == ReplEventType.Exit)
      {
        return Exit(new ReplOutput(ReplOutputType.quit, state));
      }
      else if (state.currentAppEvent?.type == ReplEventType.WriteToFile)
      {
        return Exit(new ReplOutput(ReplOutputType.outputToFile, state));
      }
      else if (state.currentAppEvent?.type == ReplEventType.WriteToClipboard)
      {
        return Exit(new ReplOutput(ReplOutputType.outputToClipboard, state));
      }

      replInterface.DrawInterface(terminal);
    }

    // This should be unreachable
    return Exit(new InvalidOperationException("This code should be unreachable."));
  }

  /// <summary>
  /// Ensure all resources are cleaned up and return the repl output.
  /// </summary>
  /// <param name="output"></param>
  /// <returns></returns>
  private Result<ReplOutput, Exception> Exit(Result<ReplOutput, Exception> output)
  {
    // Cancel all threads
    cancellationTokenSource.Cancel();
    return output;
  }
}
