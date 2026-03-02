namespace Sew;

public static class Function
{
  public static bool Tautology<T>(T args)
  {
    return true;
  }

  public static T Identity<T>(T value)
  {
    return value;
  }

  public static IEnumerable<T> UntilNull<T>(Func<T?> generator)
    where T : class
  {
    for (var element = generator(); element != null; element = generator())
    {
      yield return element;
    }
  }

  public static Func<Args, Result> Memo<Args, Result>(Func<Args, Result> func)
  {
    bool hasResult = false;
    Args? lastArgs = default;
    Result? lastResult = default;
    return (args) =>
    {
      if (!hasResult || !args!.Equals(lastArgs))
      {
        lastArgs = args;
        lastResult = func(args);
        hasResult = true;
      }
      return lastResult!;
    };
  }
}
