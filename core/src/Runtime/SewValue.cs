using CriusNyx.Util;

namespace Sew;

public abstract class SewValue { }

public class BooleanValue(bool value) : SewValue, DebugPrint
{
  public bool Value => value;

  public IEnumerable<(string, object)> EnumerateFields()
  {
    return [nameof(Value).With(Value)];
  }
}
