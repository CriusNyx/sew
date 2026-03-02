using System.Diagnostics;
using CriusNyx.Results;
using CriusNyx.Results.Extensions;

/// <summary>
/// Helper class to get data out of the clipboard.
/// </summary>
public static class Clipboard
{
  public static string GetClipboard()
  {
    return GetWlClipboard().UnwrapOr("");
  }

  private static Option<string> GetWlClipboard()
  {
    try
    {
      var process = Process.Start(
        new ProcessStartInfo("wl-paste") { RedirectStandardOutput = true }
      );
      process?.WaitForExit();
      var result = process?.StandardOutput.ReadToEnd().TrimEnd('\n');
      return result.AsOption();
    }
    catch
    {
      return Option.None<string>();
    }
  }

  public static void Paste(string value)
  {
    Process.Start(new ProcessStartInfo("wl-copy", value))?.WaitForExit();
  }
}
