using System.Drawing;

namespace Sew;

public static class DrawingExtensions
{
  public static bool Inside(this Point point, Size size)
  {
    return size.Contains(point);
  }

  public static bool Contains(this Size size, Point point)
  {
    return point.X >= 0 && point.X < size.Width && point.Y >= 0 && point.Y < size.Height;
  }
}
