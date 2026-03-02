using CriusNyx.Util;
using Superpower;

namespace Sew;

public static class SuperpowerExtensions
{
  public static TokenListParser<T, U> ThenIgnore<T, U, V>(
    this TokenListParser<T, U> source,
    TokenListParser<T, V> ignore
  )
  {
    return from src in source from ing in ignore select src;
  }

  public static TokenListParser<T, IEnumerable<U>> SeparatedBy<T, U, V>(
    this TokenListParser<T, U> source,
    TokenListParser<T, V> separator
  )
  {
    return source
      .Select(x => x.AsArray().As<IEnumerable<U>>())
      .Chain(separator, source, (op, a, b) => a!.Concat(b.AsArray()))!;
  }
}
