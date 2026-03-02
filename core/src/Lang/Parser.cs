using CriusNyx.Util;
using Superpower;
using Superpower.Parsers;

namespace Sew;

public static class SewParser
{
  public static TokenListParser<SewTokenType, StringLit> StringLitParser = Token
    .EqualTo(SewTokenType.@string)
    .Select(x => new StringLit(x));

  public static TokenListParser<SewTokenType, NumLit> NumLitParser = Token
    .EqualTo(SewTokenType.number)
    .Select(x => new NumLit(x));

  public static TokenListParser<SewTokenType, Identifier> IdentifierParser = Token
    .EqualTo(SewTokenType.symbol)
    .Select(x => new Identifier(x));

  public static TokenListParser<SewTokenType, RegexLit> RegexLitParser = Token
    .EqualTo(SewTokenType.regex)
    .Select(x => new RegexLit(x));

  public static TokenListParser<SewTokenType, RegexTransformer> RegexTransformer =
    from regex in RegexLitParser
    from str in StringLitParser
    select new RegexTransformer(regex, str);

  public static TokenListParser<SewTokenType, object> PipeParser = Token
    .EqualTo(SewTokenType.pipe)
    .Select(x => null as object)!;

  public static TokenListParser<SewTokenType, InvertExpression> InvertParser =
    from excl in Token.EqualTo(SewTokenType.exclamation)
    from expression in ExpressionParser.NotNull()
    select new InvertExpression(excl, expression);

  public static TokenListParser<SewTokenType, Argument> NamedArgumentParser =
    from identifier in IdentifierParser
    from colon in Token.EqualTo(SewTokenType.colon)
    from expression in ExpressionParser.NotNull()
    select new NamedArgument(identifier, colon, expression) as Argument;

  public static TokenListParser<SewTokenType, Argument> FlagArgumentParser =
    from colon in Token.EqualTo(SewTokenType.colon)
    from identifier in IdentifierParser
    select new FlagArgument(colon, identifier) as Argument;

  public static TokenListParser<SewTokenType, Argument> ValueArgumentParser = Parse.Ref(() =>
    PipeExpressionParser.NotNull().Select(x => new ValueArgument(x) as Argument)
  );

  public static TokenListParser<SewTokenType, Argument> ArgumentParser = Parse.OneOf(
    NamedArgumentParser.Try(),
    FlagArgumentParser,
    ValueArgumentParser
  );

  public static TokenListParser<SewTokenType, ASTNode> FunctionExpressionParser = Parse.OneOf(
    IdentifierParser.AsAST(),
    RegexLitParser.AsAST()
  );

  public static TokenListParser<SewTokenType, ASTNode> InvocationParser =
    from functionExpression in FunctionExpressionParser
    from openParen in Token.EqualTo(SewTokenType.openParen)
    from parameters in ArgumentParser
      .SeparatedBy(Token.EqualTo(SewTokenType.comma))
      .OptionalOrDefault([])
    from closedParen in Token.EqualTo(SewTokenType.closedParen)
    select new Invocation(functionExpression, openParen, parameters, closedParen) as ASTNode;

  public static TokenListParser<SewTokenType, ASTNode> UnitParser =
    from openParen in Token.EqualTo(SewTokenType.openParen)
    from expression in PipeExpressionParser.NotNull()
    from closedParen in Token.EqualTo(SewTokenType.closedParen)
    select new Unit(openParen, expression, closedParen) as ASTNode;

  public static TokenListParser<SewTokenType, ASTNode> ExpressionParser = Parse.Ref(() =>
    Parse.OneOf(
      InvocationParser.Try(),
      RegexTransformer.AsAST().Try(),
      IdentifierParser.AsAST(),
      StringLitParser.AsAST(),
      NumLitParser.AsAST(),
      RegexLitParser.AsAST(),
      InvertParser.AsAST(),
      UnitParser
    )
  );

  public static TokenListParser<SewTokenType, ASTNode> PipeExpressionParser =
    ExpressionParser.Chain(
      PipeParser,
      ExpressionParser,
      (operand, left, right) => new Pipe(left, right)
    );

  public static TokenListParser<SewTokenType, ASTNode?> SewProgram = Parse.OneOf(
    PipeExpressionParser!
      .OptionalOrDefault()
      .Select(expression => new SewProgram(expression) as ASTNode)
      .AtEnd()
  )!;

  public static TokenListParser<SewTokenType, ASTNode> AsAST<T>(
    this TokenListParser<SewTokenType, T> tokenListParser
  )
    where T : ASTNode
  {
    return tokenListParser.Select(x => x as ASTNode);
  }
}
