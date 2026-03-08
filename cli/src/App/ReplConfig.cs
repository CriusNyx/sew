using CriusNyx.Results;

namespace Sew.CLI;

/// <summary>
/// Stores configuration to initialize repl.
/// </summary>
public class ReplConfig
{
  /// <summary>
  /// Initial input for the sew program.
  /// </summary>
  public Option<string> Input = Option.None();
}
