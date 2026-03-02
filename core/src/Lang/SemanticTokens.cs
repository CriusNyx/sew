using Superpower.Model;

namespace Sew;

public enum SemanticType
{
  none,
  keyword,
  method,
  identifier,
  numLit,
  regexLit,
  stringLit,
}

public class SemanticToken(Token<SewTokenType> token, SemanticType type)
{
  public Token<SewTokenType> Token => token;
  public SemanticType Type => type;

  public static SemanticToken From(Token<SewTokenType> token)
  {
    return new SemanticToken(token, TypeFromTokenType(token.Kind));
  }

  private static SemanticType TypeFromTokenType(SewTokenType tokenType)
  {
    switch (tokenType)
    {
      case SewTokenType.number:
        return SemanticType.numLit;
      case SewTokenType.regex:
        return SemanticType.regexLit;
      case SewTokenType.@string:
        return SemanticType.stringLit;
      case SewTokenType.symbol:
        return SemanticType.identifier;
      case SewTokenType.closedParen:
      case SewTokenType.colon:
      case SewTokenType.comma:
      case SewTokenType.exclamation:
      case SewTokenType.openParen:
      case SewTokenType.pipe:
      case SewTokenType.whitespace:
      default:
        return SemanticType.none;
    }
  }
}
