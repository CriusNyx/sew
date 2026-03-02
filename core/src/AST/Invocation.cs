using CriusNyx.Util;
using Superpower.Model;

namespace Sew;

public class Invocation(
  ASTNode functionExpression,
  Token<SewTokenType> openParen,
  IEnumerable<Argument> parameters,
  Token<SewTokenType> closedParen
) : ASTNode
{
  public ASTNode FunctionExpression => functionExpression;
  public Token<SewTokenType> OpenParent => openParen;
  public IEnumerable<Argument> Parameters => parameters;
  public Token<SewTokenType> ClosedParen => closedParen;

  public override IEnumerable<(string, object)> EnumerateFields()
  {
    return
    [
      nameof(FunctionExpression).With(FunctionExpression),
      nameof(Parameters).With(Parameters),
    ];
  }

  public override object? Evaluate(ExecutionContext context)
  {
    var functionValue = FunctionExpression.Evaluate(context);
    var closure = parameters.Aggregate(
      new Closure(),
      (prev, param) => param.EvaluateClosure(context, prev)
    );
    if (functionValue is Invocable invocable)
    {
      return invocable.Invoke(closure);
    }
    return null;
  }

  public override IEnumerable<Token<SewTokenType>> GetSourceTokens()
  {
    return FunctionExpression
      .GetSourceTokens()
      .Concat([openParen])
      .Concat(Parameters.SelectMany(param => param.GetSourceTokens()))
      .Concat([closedParen]);
  }

  public override IEnumerable<SemanticToken> GetSemanticTokens()
  {
    if (FunctionExpression is Identifier identifier)
    {
      return new SemanticToken(identifier.Token, SemanticType.method).ThenConcat(
        Parameters.SelectMany((x) => x.GetSemanticTokens())
      );
    }
    else
    {
      return FunctionExpression
        .GetSemanticTokens()
        .Concat(Parameters.SelectMany(x => x.GetSemanticTokens()));
    }
  }

  public override bool ASTEquivalent(ASTNode other, List<ASTEquivalentError> errors)
  {
    if (other is Invocation invocation)
    {
      return FunctionExpression.ASTEquivalent(invocation.FunctionExpression, errors)
        && Parameters.ASTEquivalent(invocation.Parameters, errors);
    }
    errors.Add(ASTEquivalentError.TypeError(this, other));
    return false;
  }
}
