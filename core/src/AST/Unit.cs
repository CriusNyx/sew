using CommandLine;
using CriusNyx.Util;
using Superpower.Model;

namespace Sew;

public class Unit(
  Token<SewTokenType> openParen,
  ASTNode expression,
  Token<SewTokenType> closedParen
) : ASTNode
{
  public Token<SewTokenType> OpenParen => openParen;
  public ASTNode Expression => expression;
  public Token<SewTokenType> ClosedParen => closedParen;

  public override IEnumerable<(string, object)> EnumerateFields()
  {
    return [nameof(Expression).With(Expression)];
  }

  public override object? Evaluate(ExecutionContext context)
  {
    var body = Expression.Evaluate(context);
    if (body is HasTransformer transformer)
    {
      return new FunctionTransformer(
        (_) => (values) => values.SelectMany((str) => transformer.Transform([str]))
      );
    }
    else
    {
      return body;
    }
  }

  public override IEnumerable<Token<SewTokenType>> GetSourceTokens()
  {
    return openParen.ThenConcat(Expression.GetSourceTokens()).Concat([closedParen]);
  }

  public override IEnumerable<SemanticToken> GetSemanticTokens()
  {
    return Expression.GetSemanticTokens();
  }

  public override bool ASTEquivalent(ASTNode other, List<ASTEquivalentError> errors)
  {
    if (other is Unit unit)
    {
      return Expression.ASTEquivalent(unit.Expression, errors);
    }
    errors.Add(ASTEquivalentError.TypeError(this, other));
    return false;
  }
}
