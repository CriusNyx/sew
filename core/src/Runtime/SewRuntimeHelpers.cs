using System.Collections;
using CriusNyx.Results;

public static class SewRuntimeHelpers
{
  public static object ApplyValueTransform<T>(object to, Func<T, T> transformation)
  {
    if (to is T t)
    {
      return transformation(t)!;
    }
    if (to is IEnumerable enumerable)
    {
      return enumerable.Select(value => ApplyValueTransform(value, transformation));
    }
    return to;
  }

  public static object ApplyFilter<T>(object to, Func<T, bool> filter)
  {
    IEnumerable ApplyEnumerable(IEnumerable enumerable)
    {
      foreach (var element in enumerable)
      {
        if (element is T t)
        {
          if (filter(t))
          {
            yield return t;
          }
        }
        else
        {
          yield return ApplyFilter(element, filter);
        }
      }
    }

    if (to is IEnumerable enumerable)
    {
      return ApplyEnumerable(enumerable);
    }
    return to;
  }

  private static IEnumerable Select(this IEnumerable enumerable, Func<object, object> application)
  {
    foreach (var element in enumerable)
    {
      yield return application(element);
    }
  }
}
