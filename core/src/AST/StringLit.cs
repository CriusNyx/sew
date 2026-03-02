using System.Text.RegularExpressions;
using CriusNyx.Util;
using Superpower.Model;

namespace Sew;

public class StringLit(Token<SewTokenType> token) : ASTNode
{
  public Token<SewTokenType> Token => token;
  public string Literal =>
    Token
      .ToStringValue()
      .Transform(x => x.Substring(1, x.Length - 2))
      .Transform(x => Regex.Unescape(x));

  public override object? Evaluate(ExecutionContext context)
  {
    return Literal;
  }

  public override IEnumerable<(string, object)> EnumerateFields()
  {
    return [nameof(Literal).With(Literal)];
  }

  public override IEnumerable<Token<SewTokenType>> GetSourceTokens()
  {
    return [Token];
  }

  public override IEnumerable<SemanticToken> GetSemanticTokens()
  {
    return [new SemanticToken(Token, SemanticType.stringLit)];
  }

  public override bool ASTEquivalent(ASTNode other, List<ASTEquivalentError> errors)
  {
    var stringLit = other as StringLit;
    if (stringLit == null)
    {
      errors.Add(ASTEquivalentError.TypeError(this, other));
      return false;
    }

    if (Token.ToStringValue() != stringLit.Token.ToStringValue())
    {
      errors.Add(new ASTEquivalentError(this, other, "String lit values do not match"));
      return false;
    }

    return true;
  }
}
