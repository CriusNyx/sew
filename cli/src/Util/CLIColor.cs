/// <summary>
/// Helper class for adding colors to the CLI.
/// </summary>
public static class CLIColor
{
  public const string BOLD = "\x1b[1m";
  public const string END_BOLD = "\x1b[22m";

  public static string Bold(this string source)
  {
    return $"{BOLD}{source}{END_BOLD}";
  }
}
