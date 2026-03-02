using CommandLine;
using CriusNyx.Util;
using Superpower.Model;

namespace Sew;

public class SewProgram(ASTNode? body) : ASTNode
{
  public ASTNode? Body => body;

  public override IEnumerable<(string, object)> EnumerateFields()
  {
    return [nameof(Body).With(Body)!];
  }

  public override object? Evaluate(ExecutionContext context)
  {
    if (Body == null)
    {
      return FunctionTransformer.FromMap(Function.Identity);
    }
    return Body?.Evaluate(context);
  }

  public override IEnumerable<Token<SewTokenType>> GetSourceTokens()
  {
    return Body?.GetSourceTokens() ?? [];
  }

  public override IEnumerable<SemanticToken> GetSemanticTokens()
  {
    return Body?.GetSemanticTokens() ?? [];
  }

  public override bool ASTEquivalent(ASTNode other, List<ASTEquivalentError> errors)
  {
    if (other is SewProgram prog)
    {
      if (Body == null && prog.Body == null)
      {
        return true;
      }
      return Body?.ASTEquivalent(prog.Body!, errors) ?? false;
    }
    errors.Add(ASTEquivalentError.TypeError(this, other));
    return false;
  }
}
