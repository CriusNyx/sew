using CommandLine;
using CriusNyx.Results;
using CriusNyx.Util;
using Superpower;
using Superpower.Model;

namespace Sew;

public class SewLangOutput(ASTNode ast, IEnumerable<string> output)
{
  public ASTNode AST => ast;
  public IEnumerable<string> Output => output;
}

public class SewError : DebugPrint
{
  Span span;
  string error;

  public SewError(Span span, string error)
  {
    this.span = span;
    this.error = error;
  }

  public IEnumerable<(string, object)> EnumerateFields()
  {
    return [nameof(span).With(span), nameof(error).With(error)];
  }

  public override string ToString()
  {
    return $"({span.Start} {span.Length}): {error}";
  }
}

public class SewLangException(IEnumerable<SewError> errors) : DebugPrint
{
  public IEnumerable<SewError> Errors => errors;

  public IEnumerable<(string, object)> EnumerateFields()
  {
    return [nameof(Errors).With(Errors)];
  }

  public override string ToString()
  {
    return Errors.Select(x => x.ToString() ?? "").StringJoin("\n");
  }
}

public class SewLang
{
  public static Result<TokenList<SewTokenType>, SewLangException> Tokenize(string source)
  {
    return SewTokenizer.Tokenize(source);
  }

  public static Result<ASTNode, SewLangException> Parse(TokenList<SewTokenType> tokens)
  {
    var parsed = SewParser.SewProgram.TryParse(tokens);
    if (!parsed.HasValue)
    {
      return new SewLangException([
        new SewError(new(parsed.ErrorPosition.Absolute, 1), parsed.FormatErrorMessageFragment()),
      ]);
    }
    return parsed.Value!;
  }

  public static Result<ASTNode, SewLangException> Parse(string sewSource)
  {
    return Tokenize(sewSource).AndThen(Parse);
  }

  public static Result<SewLangOutput, SewLangException> Eval(ASTNode ast, string input)
  {
    var context = new ExecutionContext();
    var evaluation = ast.Evaluate(context);

    if (evaluation is HasTransformer transformer)
    {
      try
      {
        var output = transformer.Transform([input]);
        return new SewLangOutput(ast, output);
      }
      catch (Exception)
      {
        return new SewLangException([new SewError(new Span(), "Runtime exception")]);
      }
    }
    else
    {
      return new SewLangException([
        new SewError(new Span(), $"Value {evaluation!.Debug()} is not valid"),
      ]);
    }
  }

  public static Result<SewLangOutput, SewLangException> Eval(string input, string sewProgram)
  {
    return Parse(sewProgram).AndThen((ast) => Eval(ast, input));
  }
}
