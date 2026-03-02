namespace Sew;

[AttributeUsage(AttributeTargets.Method)]
public class SewMethodAttribute(IEnumerable<string> aliases) : Attribute
{
  public IEnumerable<string> Aliases => aliases;

  public SewMethodAttribute(params string[] aliases)
    : this(aliases as IEnumerable<string>) { }
}
