namespace Sew;

public enum LogSeverity
{
  Debug,
  Log,
  Error,
}

public class Logger
{
  private List<(LogSeverity, object)> messages = new List<(LogSeverity, object)>();
  public IEnumerable<(LogSeverity severity, object o)> Messages => messages;
  public static Logger Instance { get; } = new Logger();

  public void Append(LogSeverity severity, object o)
  {
    messages.Add((severity, o));
  }

  public void _Debug(object o) => Append(LogSeverity.Debug, o);

  public static void Debug(object o) => Instance._Debug(o);

  public void _Log(object o) => Append(LogSeverity.Log, o);

  public static void Log(object o) => Instance._Log(o);

  public void _Error(object o) => Append(LogSeverity.Error, o);

  public static void Error(object o) => Instance._Error(o);
}
