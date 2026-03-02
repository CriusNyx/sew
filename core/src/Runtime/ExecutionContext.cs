namespace Sew;

public class ExecutionContext
{
  public object? Deref(string identifier)
  {
    return SewRuntime.DerefRuntimeValue(identifier);
  }
}
