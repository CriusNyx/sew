namespace Sew;

public delegate bool Predicate(string value);

public interface HasPredicate
{
  public Predicate GetPredicate();
}

public class PredicateFunc : HasTransformer, Invocable, Invertible, HasPredicate
{
  Func<Closure, Predicate> predicate;
  Closure closure;

  public PredicateFunc(Func<Closure, Predicate> predicate, Closure? closure = null)
  {
    this.predicate = predicate;
    this.closure = closure ?? new Closure([], []);
  }

  public object Invoke(Closure closure)
  {
    return new PredicateFunc(predicate, this.closure.Combine(closure));
  }

  public override IEnumerable<string> Transform(IEnumerable<string> values)
  {
    return values.Where(GetPredicate().Invoke);
  }

  public static PredicateFunc From(Predicate predicateFunc)
  {
    return new PredicateFunc((_) => predicateFunc);
  }

  public SewValue Invert()
  {
    var applied = predicate(closure);
    return From((x) => !applied(x));
  }

  public Predicate GetPredicate()
  {
    return predicate(closure);
  }
}
