using CriusNyx.Util;
using Superpower.Model;

namespace Sew;

public class InvertExpression(Token<SewTokenType> @operator, ASTNode expression) : ASTNode
{
  public Token<SewTokenType> Operator => @operator;
  public ASTNode Expression => expression;

  public override IEnumerable<(string, object)> EnumerateFields()
  {
    return [nameof(Expression).With(Expression)];
  }

  public override object? Evaluate(ExecutionContext context)
  {
    var value = expression.Evaluate(context);
    if (value is Invertible invertible)
    {
      return invertible.Invert();
    }

    return null;
  }

  public override IEnumerable<Token<SewTokenType>> GetSourceTokens()
  {
    return @operator.ThenConcat(expression.GetSourceTokens());
  }

  public override IEnumerable<SemanticToken> GetSemanticTokens()
  {
    return Expression.GetSemanticTokens();
  }

  public override bool ASTEquivalent(ASTNode other, List<ASTEquivalentError> errors)
  {
    if (other is InvertExpression invert)
    {
      return Expression.ASTEquivalent(invert.Expression, errors);
    }
    errors.Add(ASTEquivalentError.TypeError(this, other));
    return false;
  }
}
