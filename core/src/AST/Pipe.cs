using CriusNyx.Util;
using Superpower.Model;

namespace Sew;

public class Pipe(ASTNode left, ASTNode right) : ASTNode, DebugPrint
{
  public ASTNode Left => left;
  public ASTNode Right => right;

  public override IEnumerable<(string, object)> EnumerateFields()
  {
    return [nameof(Left).With(Left), nameof(Right).With(Right)];
  }

  public override object? Evaluate(ExecutionContext context)
  {
    var leftVal = Left.Evaluate(context);
    var rightVal = Right.Evaluate(context);
    if (leftVal is HasTransformer lt && rightVal is HasTransformer rt)
    {
      return new FunctionTransformer((_) => (values) => rt.Transform(lt.Transform(values)));
    }
    return rightVal;
  }

  public override IEnumerable<Token<SewTokenType>> GetSourceTokens()
  {
    return Left.GetSourceTokens().Concat(Right.GetSourceTokens());
  }

  public override IEnumerable<SemanticToken> GetSemanticTokens()
  {
    return [.. Left.GetSemanticTokens(), .. Right.GetSemanticTokens()];
  }

  public override bool ASTEquivalent(ASTNode other, List<ASTEquivalentError> errors)
  {
    if (other is Pipe pipe)
    {
      return Left.ASTEquivalent(pipe.Left, errors) && Right.ASTEquivalent(pipe.Right, errors);
    }

    errors.Add(ASTEquivalentError.TypeError(this, other));
    return false;
  }
}
