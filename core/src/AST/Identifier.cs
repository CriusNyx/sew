using CriusNyx.Util;
using Superpower.Model;

namespace Sew;

public class Identifier(Token<SewTokenType> symbol) : ASTNode
{
  public Token<SewTokenType> Token => symbol;
  public string Source => Token.ToStringValue();

  public override IEnumerable<(string, object)> EnumerateFields()
  {
    return [nameof(Token).With(Token)];
  }

  public override object? Evaluate(ExecutionContext context)
  {
    return context.Deref(Source);
  }

  public override IEnumerable<Token<SewTokenType>> GetSourceTokens()
  {
    return [Token];
  }

  public override IEnumerable<SemanticToken> GetSemanticTokens()
  {
    return [new SemanticToken(Token, SemanticType.identifier)];
  }

  public override bool ASTEquivalent(ASTNode other, List<ASTEquivalentError> errors)
  {
    var ident = other as Identifier;
    if (ident == null)
    {
      errors.Add(ASTEquivalentError.TypeError(this, other));
      return false;
    }

    if (Source != ident.Source)
    {
      errors.Add(new ASTEquivalentError(this, other, "Symbols do not match."));
      return false;
    }

    return true;
  }
}
