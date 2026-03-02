using System.Drawing;
using CriusNyx.Util;
using Sharpie;
using Sharpie.Abstractions;

namespace Sew.CLI;

public struct Padding
{
  public int top;
  public int bottom;
  public int left;
  public int right;

  public Rectangle Area(Size size) => Area(Point.Empty, size);

  public Rectangle Area(Point point, Size size)
  {
    return new Rectangle(
      new Point(point.X + left, point.Y + top),
      new Size(size.Width - left - right, size.Height - top - bottom)
    );
  }
}

public struct View
{
  public IScreen screen;

  public Rectangle rectangle;

  public View(IScreen screen, Rectangle rectangle)
  {
    this.screen = screen;
    this.rectangle = rectangle;
  }

  public IWindow Window() => screen.Window(rectangle);
}

public static class CursesExtensions
{
  public static View View(this IScreen screen, Padding? padding = null)
  {
    var rectangle = padding.OrDefault(new())!.Value.Area(screen.Size);
    return new(screen, rectangle);
  }

  public static View View(this View view, Padding padding)
  {
    var rectangle = padding.Area(view.rectangle.Location, view.rectangle.Size);
    return new(view.screen, rectangle);
  }

  public static View View(this View view, Rectangle rectangle)
  {
    return new(view.screen, rectangle);
  }

  public static (View top, View bottom) SplitV(this View view, int splitPoint)
  {
    if (splitPoint < 0)
    {
      return view.SplitV(view.rectangle.Height + splitPoint);
    }
    return view.View(
        new Rectangle(
          view.rectangle.Location.X,
          view.rectangle.Location.Y,
          view.rectangle.Width,
          splitPoint
        )
      )
      .With(
        view.View(
          new Rectangle(
            view.rectangle.Location.X,
            view.rectangle.Location.Y + splitPoint,
            view.rectangle.Width,
            view.rectangle.Height - splitPoint
          )
        )
      );
  }

  public static (View left, View right) SplitH(this View view, int splitPoint)
  {
    if (splitPoint < 0)
    {
      return view.SplitH(view.rectangle.Width + splitPoint);
    }
    return view.View(
        new Rectangle(
          view.rectangle.Location.X,
          view.rectangle.Location.Y,
          splitPoint,
          view.rectangle.Height
        )
      )
      .With(
        view.View(
          new Rectangle(
            view.rectangle.Location.X + splitPoint,
            view.rectangle.Location.Y,
            view.rectangle.Width - splitPoint,
            view.rectangle.Height
          )
        )
      );
  }

  public static string ToEventString(this Sharpie.Event @event)
  {
    return $"{@event} eventClass: {@event.GetType().Name} type: {@event.Type} key: {@event.As<KeyEvent>()?.Key} charValue: {@event.As<KeyEvent>()?.Char.Value} char: {@event.As<KeyEvent>()?.Char.ToString()} name: {@event.As<KeyEvent>()?.Name}";
  }
}
