using CriusNyx.Util;
using Superpower.Model;

namespace Sew;

public class NumLit(Token<SewTokenType> token) : ASTNode
{
  public Token<SewTokenType> Token => token;
  public decimal Value => token.ToStringValue().Transform(str => decimal.Parse(str));

  public override IEnumerable<(string, object)> EnumerateFields()
  {
    return [nameof(Value).With(Value)];
  }

  public override object? Evaluate(ExecutionContext context)
  {
    return Value;
  }

  public override IEnumerable<Token<SewTokenType>> GetSourceTokens()
  {
    return [Token];
  }

  public override IEnumerable<SemanticToken> GetSemanticTokens()
  {
    return [new SemanticToken(Token, SemanticType.numLit)];
  }

  public override bool ASTEquivalent(ASTNode other, List<ASTEquivalentError> errors)
  {
    var numLit = other as NumLit;
    if (numLit == null)
    {
      errors.Add(ASTEquivalentError.TypeError(this, other));
      return false;
    }

    if (Token.ToStringValue() != numLit.Token.ToStringValue())
    {
      errors.Add(new ASTEquivalentError(this, other, "numLit values do not match"));
      return false;
    }

    return true;
  }
}
