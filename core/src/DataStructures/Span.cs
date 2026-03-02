using System.Security.Cryptography.X509Certificates;
using Microsoft.CodeAnalysis;

namespace Sew;

public struct Span
{
  public readonly int Start;
  public readonly int Length;
  public int End => Start + Length;

  public Span(int Start, int Length)
  {
    this.Start = Start;
    this.Length = Length;
  }

  public bool Contains(int position)
  {
    return position >= Start && position < Start + Length;
  }

  public bool Intersects(Span other)
  {
    var left = this;
    var right = other;
    if (right.Start < left.Start)
    {
      (left, right) = (right, left);
    }
    return right.Start < left.End;
  }

  public override string ToString()
  {
    return $"Span({Start}, {Length})";
  }
}
