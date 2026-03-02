namespace Sew.CLI;

public enum ReplOutputType
{
  quit,
  outputToFile,
  outputToClipboard,
}

public class ReplOutput(ReplOutputType type, State state)
{
  public ReplOutputType Type => type;
  public State State => state;
}
