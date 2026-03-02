namespace Sew;

public delegate IEnumerable<string> Transformer(IEnumerable<string> input);

public abstract class HasTransformer : SewValue
{
  public abstract IEnumerable<string> Transform(IEnumerable<string> values);
}

public class FunctionTransformer : HasTransformer, Invocable
{
  Func<Closure, Transformer> Function;
  Closure closure;

  public FunctionTransformer(Func<Closure, Transformer> function, Closure closure = null!)
  {
    Function = function;
    this.closure = closure ?? new Closure();
  }

  public override IEnumerable<string> Transform(IEnumerable<string> values)
  {
    return Function(closure)(values);
  }

  public static FunctionTransformer FromMap(Func<string, string> mapFunc)
  {
    return new FunctionTransformer((_) => (source) => source.Select(mapFunc));
  }

  public object Invoke(Closure closure)
  {
    return new FunctionTransformer(Function, this.closure.Combine(closure));
  }
}
