using CriusNyx.Util;
using Superpower.Model;

namespace Sew;

public abstract class Argument : ASTNode
{
  public abstract Closure EvaluateClosure(ExecutionContext context, Closure previous);
}

public class ValueArgument(ASTNode expression) : Argument
{
  public ASTNode Expression => expression;

  public override IEnumerable<(string, object)> EnumerateFields()
  {
    return [nameof(Expression).With(Expression)];
  }

  public override object? Evaluate(ExecutionContext context)
  {
    return Expression.Evaluate(context);
  }

  public override Closure EvaluateClosure(ExecutionContext context, Closure previous)
  {
    var value = Evaluate(context);
    return previous.Combine(new Closure([value!]));
  }

  public override IEnumerable<Token<SewTokenType>> GetSourceTokens()
  {
    return expression.GetSourceTokens();
  }

  public override IEnumerable<SemanticToken> GetSemanticTokens()
  {
    return Expression.GetSemanticTokens();
  }

  public override bool ASTEquivalent(ASTNode other, List<ASTEquivalentError> errors)
  {
    if (other is ValueArgument valueArgument)
    {
      return Expression.ASTEquivalent(valueArgument.Expression, errors);
    }
    errors.Add(ASTEquivalentError.TypeError(this, other));
    return false;
  }
}

public class NamedArgument(Identifier identifier, Token<SewTokenType> colon, ASTNode expression)
  : Argument
{
  public Identifier Identifier => identifier;
  public Token<SewTokenType> Colon => colon;
  public ASTNode Expression => expression;

  public override IEnumerable<(string, object)> EnumerateFields()
  {
    return [nameof(Identifier).With(Identifier), nameof(Expression).With(Expression)];
  }

  public override object? Evaluate(ExecutionContext context)
  {
    return Expression.Evaluate(context);
  }

  public override Closure EvaluateClosure(ExecutionContext context, Closure previous)
  {
    var value = Evaluate(context);
    return previous.Combine(new Closure([], [Identifier.Source.With(value)!]));
  }

  public override IEnumerable<Token<SewTokenType>> GetSourceTokens()
  {
    return identifier.GetSourceTokens().Concat([colon]).Concat(expression.GetSourceTokens());
  }

  public override IEnumerable<SemanticToken> GetSemanticTokens()
  {
    return [.. Identifier.GetSemanticTokens(), .. Expression.GetSemanticTokens()];
  }

  public override bool ASTEquivalent(ASTNode other, List<ASTEquivalentError> errors)
  {
    if (other is NamedArgument named)
    {
      return Identifier.ASTEquivalent(named.Identifier, errors)
        && Expression.ASTEquivalent(named.Expression, errors);
    }
    errors.Add(ASTEquivalentError.TypeError(this, other));
    return false;
  }
}

public class FlagArgument(Token<SewTokenType> colon, Identifier identifier) : Argument
{
  public Identifier Identifier => identifier;

  public override IEnumerable<(string, object)> EnumerateFields()
  {
    return [nameof(Identifier).With(Identifier)];
  }

  public override object? Evaluate(ExecutionContext context)
  {
    return new BooleanValue(true);
  }

  public override Closure EvaluateClosure(ExecutionContext context, Closure previous)
  {
    var value = Evaluate(context);
    return previous.Combine(new Closure([], [Identifier.Source.With(value)!]));
  }

  public override IEnumerable<Token<SewTokenType>> GetSourceTokens()
  {
    return colon.ThenConcat(identifier.GetSourceTokens());
  }

  public override IEnumerable<SemanticToken> GetSemanticTokens()
  {
    return Identifier.GetSemanticTokens();
  }

  public override bool ASTEquivalent(ASTNode other, List<ASTEquivalentError> errors)
  {
    if (other is FlagArgument flag)
    {
      return Identifier.ASTEquivalent(flag.Identifier, errors);
    }
    errors.Add(ASTEquivalentError.TypeError(this, other));
    return false;
  }
}
