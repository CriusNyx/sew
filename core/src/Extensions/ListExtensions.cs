public static class ListExtensions
{
  public static IEnumerable<(T first, T next)> WithNext<T>(this IEnumerable<T> values)
  {
    bool passedFirst = false;
    T? previous = default!;
    foreach (var value in values)
    {
      if (passedFirst)
      {
        yield return (previous, value);
      }
      else
      {
        passedFirst = true;
      }
      previous = value;
    }
  }
}
