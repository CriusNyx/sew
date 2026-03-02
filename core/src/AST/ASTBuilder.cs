using CriusNyx.Util;
using Superpower.Model;

namespace Sew;

public static class SewBuilder
{
  public static Token<SewTokenType> Token(SewTokenType kind, string source)
  {
    return new Token<SewTokenType>(kind, new TextSpan(source));
  }

  public static Token<SewTokenType> Examination => Token(SewTokenType.exclamation, "!");
  public static Token<SewTokenType> OpenParen => Token(SewTokenType.openParen, "(");
  public static Token<SewTokenType> ClosedParen => Token(SewTokenType.closedParen, ")");
  public static Token<SewTokenType> Colon => Token(SewTokenType.colon, ":");

  public static Identifier Ident(string symbol)
  {
    return new Identifier(Token(SewTokenType.symbol, symbol));
  }

  public static InvertExpression Inv(ASTNode inner)
  {
    return new InvertExpression(Examination, inner);
  }

  public static Invocation Invoke(ASTNode functionExpression, params Argument[] args)
  {
    return new Invocation(functionExpression, OpenParen, args, ClosedParen);
  }

  public static Invocation Invoke(string functionName, params Argument[] args)
  {
    return Invoke(Ident(functionName), args);
  }

  public static Argument ValArg(ASTNode expression)
  {
    return new ValueArgument(expression);
  }

  public static Argument ValArg(string valueName)
  {
    return ValArg(Ident(valueName));
  }

  public static Argument Named(string symbol, ASTNode expression)
  {
    return new NamedArgument(Ident(symbol), Colon, expression);
  }

  public static Argument FlagArg(string symbol)
  {
    return new FlagArgument(Colon, Ident(symbol));
  }

  public static NumLit Num(string source)
  {
    return new NumLit(Token(SewTokenType.number, source));
  }

  public static ASTNode Pipe(params ASTNode[] nodes)
  {
    return nodes
      .Skip(1)
      .Aggregate(nodes.First(), (ASTNode prev, ASTNode curr) => new Pipe(prev, curr));
  }

  /// <summary>
  /// Should include forward slashes.
  /// </summary>
  /// <param name="source"></param>
  /// <returns></returns>
  public static ASTNode RegexLit(string source)
  {
    return new RegexLit(Token(SewTokenType.regex, source));
  }

  /// <summary>
  /// Should include forward slashes.
  /// </summary>
  /// <param name="source"></param>
  /// <returns></returns>
  public static ASTNode RegexLit(string source, string transformer)
  {
    return new RegexTransformer(
      RegexLit(source).AsNotNull<RegexLit>(),
      StrLit(transformer).AsNotNull<StringLit>()
    );
  }

  public static ASTNode Prog(ASTNode? body)
  {
    return new SewProgram(body);
  }

  /// <summary>
  /// Should include quotes.
  /// </summary>
  /// <param name="source"></param>
  /// <returns></returns>
  public static ASTNode StrLit(string source)
  {
    return new StringLit(Token(SewTokenType.@string, source));
  }

  public static ASTNode Unit(ASTNode inner)
  {
    return new Sew.Unit(OpenParen, inner, ClosedParen);
  }
}
