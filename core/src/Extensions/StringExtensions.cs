using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;

namespace Sew;

public static class StringExtensions
{
  public static Size Measure(this string source)
  {
    var lines = source.Split("\n");
    var width = lines.Max(x => x.Length);
    int height = lines.Length;
    return new Size(width, height);
  }

  public static string SpanReplace(this string source, Span span, string replacement)
  {
    return source.Substring(0, span.Start) + replacement + source.Substring(span.End);
  }

  public static string SpanReplace(
    this string source,
    IEnumerable<(Span span, string replacement)> values
  )
  {
    var sorted = values.OrderBy(x => x.span.Start);
    var valid = sorted.WithNext().All((pair) => !pair.first.span.Intersects(pair.next.span));
    if (!valid)
    {
      throw new InvalidOperationException("Spans must not intersect");
    }
    return sorted
      .Reverse()
      .Aggregate(source, (prev, curr) => prev.SpanReplace(curr.span, curr.replacement));
  }
}
