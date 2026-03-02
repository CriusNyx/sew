namespace Sew;

public class Thunk<T>
{
  public T Value
  {
    get
    {
      if (!hasValue)
      {
        value = generator();
      }
      return value;
    }
  }

  bool hasValue = false;
  T value = default!;
  Func<T> generator;

  public Thunk(Func<T> generator)
  {
    this.generator = generator;
  }
}
