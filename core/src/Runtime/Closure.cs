using CriusNyx.Util;

namespace Sew;

public class Closure(object[] orderedArgs = null!, (string name, object value)[] namedArgs = null!)
{
  object[] _orderedArgs => orderedArgs ?? [];
  (string name, object value)[] _namedArgs => namedArgs ?? [];

  public IEnumerable<object> OrderedArgs => _orderedArgs;
  public IEnumerable<(string name, object value)> NamedArgs => _namedArgs;

  public Closure Combine(Closure other)
  {
    return new Closure(
      _orderedArgs.Expand(other._orderedArgs),
      _namedArgs.Expand(other._namedArgs)
    );
  }

  public object?[] Unwrap(params string[] argumentNames)
  {
    Dictionary<string, object?> values = argumentNames.ToDictionary(
      Function.Identity,
      (_) => null as object
    )!;

    void Load(IEnumerable<(string, object)> namedValues)
    {
      foreach (var (name, value) in namedValues)
      {
        values[name] = value;
      }
    }

    Load(argumentNames.InnerZip(_orderedArgs));
    Load(_namedArgs);

    return argumentNames.Select(x => values.Safe(x)).ToArray();
  }

  public IEnumerable<(string name, object value)> UnwrapAll()
  {
    return _orderedArgs.Select(arg => "".With(arg)).Concat(_namedArgs);
  }

  public IEnumerable<T> Params<T>()
  {
    return _orderedArgs.WhereAs<T>();
  }

  public object? GetNamedValue(params string[] candidates)
  {
    foreach (var element in _namedArgs.Reverse())
    {
      if (candidates.Contains(element.name))
      {
        return element.value;
      }
    }
    return null;
  }

  public T GetNamedValue<T>(params string[] candidates)
  {
    foreach (var element in _namedArgs.Reverse())
    {
      if (candidates.Contains(element.name))
      {
        return element.value.As<T>()!;
      }
    }
    return default!;
  }

  public bool GetFlag(params string[] candidates)
  {
    return GetNamedValue(candidates) is BooleanValue { Value: true };
  }
}
