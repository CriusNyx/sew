using CriusNyx.Util;
using Superpower.Model;

namespace Sew;

public class ASTEquivalentError(ASTNode self, ASTNode other, string message)
{
  public ASTNode Self => self;
  public ASTNode Other => other;
  public string Message => message;

  public static ASTEquivalentError TypeError(ASTNode self, ASTNode other)
  {
    return new ASTEquivalentError(self, other, "Node types do not match");
  }
}

public abstract class ASTNode : DebugPrint
{
  public abstract IEnumerable<(string, object)> EnumerateFields();

  public abstract object? Evaluate(ExecutionContext context);

  public abstract IEnumerable<Token<SewTokenType>> GetSourceTokens();

  public abstract IEnumerable<SemanticToken> GetSemanticTokens();

  public abstract bool ASTEquivalent(ASTNode other, List<ASTEquivalentError> errors);
}

public static class ASTExtensions
{
  public static bool ASTEquivalent<T>(
    this IEnumerable<T> left,
    IEnumerable<T> right,
    List<ASTEquivalentError> errors
  )
    where T : ASTNode
  {
    if (left.Count() == right.Count())
    {
      return left.Zip(right).All((pair) => pair.First.ASTEquivalent(pair.Second, errors));
    }
    errors.Add(new ASTEquivalentError(null!, null!, "Lengths do not match"));
    return false;
  }
}
