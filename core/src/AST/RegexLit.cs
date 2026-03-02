using System.Text.RegularExpressions;
using CriusNyx.Results;
using CriusNyx.Results.Extensions;
using CriusNyx.Util;
using Superpower.Model;

namespace Sew;

public class RegexLit(Token<SewTokenType> token) : ASTNode
{
  public Token<SewTokenType> Token => token;
  public string Source => Token.ToStringValue();
  public string RegexSource => Source.Transform(src => src.Substring(1, src.Length - 2));

  public override object? Evaluate(ExecutionContext context)
  {
    return new FunctionTransformer(
      (closure) =>
        (strings) =>
        {
          var args = closure.Unwrap("transformer").GetEnumerator();
          var first = args.Consume<HasTransformer>();
          if (first is HasTransformer hasTransformer)
          {
            var transformer = (string str) => hasTransformer.Transform([str]).StringJoin();
            return CreateRegex()
              .Map(regex => strings.Select(x => RegexSpanReplace(x, regex, transformer)))
              .UnwrapOr(strings);
          }
          else
          {
            return CreateRegex()
              .Map(regex => strings.Where(str => regex.IsMatch(str)))
              .UnwrapOr(strings);
          }
        }
    );
  }

  private string RegexSpanReplace(string source, Regex regex, Func<string, string> transform)
  {
    var matches = regex.Matches(source);
    var replacements = matches.Select(x => new Span(x.Index, x.Length).With(transform(x.Value)));
    return source.SpanReplace(replacements);
  }

  public Option<Regex> CreateRegex(RegexOptions options = RegexOptions.None)
  {
    try
    {
      return new Regex(RegexSource, options).AsOption();
    }
    catch
    {
      return Option.None<Regex>();
    }
  }

  public override IEnumerable<(string, object)> EnumerateFields()
  {
    return [nameof(Token).With(Token)];
  }

  public override IEnumerable<Token<SewTokenType>> GetSourceTokens()
  {
    return [Token];
  }

  public override IEnumerable<SemanticToken> GetSemanticTokens()
  {
    return [new SemanticToken(Token, SemanticType.regexLit)];
  }

  public override bool ASTEquivalent(ASTNode other, List<ASTEquivalentError> errors)
  {
    var regexLit = other as RegexLit;
    if (regexLit == null)
    {
      errors.Add(ASTEquivalentError.TypeError(this, other));
      return false;
    }

    if (Token.ToStringValue() != regexLit.Token.ToStringValue())
    {
      errors.Add(new ASTEquivalentError(this, other, "Regex lit values do not match"));
      return false;
    }

    return true;
  }
}
