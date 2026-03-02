using CriusNyx.Util;
using Superpower;
using Superpower.Model;
using Superpower.Tokenizers;
using SIdentifier = Superpower.Parsers.Identifier;
using SSpan = Superpower.Parsers.Span;

namespace Sew;

public enum SewTokenType
{
  // Non semantic
  whitespace,

  // Symbols
  exclamation,
  pipe,
  openParen,
  closedParen,
  comma,
  colon,

  // Strings and regex
  @string,
  regex,

  // Numbers
  number,

  // Symbols
  symbol,
}

public static class SewTokenizer
{
  public static TextParser<TextSpan> CreateStringParser(char terminator)
  {
    return (textSpan) =>
    {
      // Check if the span starts with the terminator.
      var first = textSpan.ConsumeChar();
      if (!(first.HasValue && first.Value == terminator))
      {
        return Result.Empty<TextSpan>(textSpan);
      }

      // Consume the rest of the string.
      var current = first.Remainder;
      for (var next = current.ConsumeChar(); next.HasValue; next = current.ConsumeChar())
      {
        // Consume End of Text.
        if (!next.HasValue)
        {
          return Result.Empty<TextSpan>(textSpan);
        }

        // Consume end of string.
        current = next.Remainder;
        if (next.Value == terminator)
        {
          var output = new TextSpan(
            textSpan.Source.NotNull(),
            textSpan.Position,
            current.Position.Absolute - textSpan.Position.Absolute
          );
          return Result.Value(output, output, current);
        }
        // Consume escape character.
        if (next.Value == '\\')
        {
          var escaped = current.ConsumeChar();
          if (!escaped.HasValue)
          {
            return Result.Empty<TextSpan>(textSpan);
          }
          current = escaped.Remainder;
        }
      }
      return Result.Empty<TextSpan>(textSpan);
    };
  }

  private static Tokenizer<SewTokenType> Tokenizer = new TokenizerBuilder<SewTokenType>()
    .Ignore(SSpan.WhiteSpace)
    // Language Symbols
    .Match(SSpan.EqualTo("!"), SewTokenType.exclamation)
    .Match(SSpan.EqualTo("|"), SewTokenType.pipe)
    .Match(SSpan.EqualTo("("), SewTokenType.openParen)
    .Match(SSpan.EqualTo(")"), SewTokenType.closedParen)
    .Match(SSpan.EqualTo(","), SewTokenType.comma)
    .Match(SSpan.EqualTo(":"), SewTokenType.colon)
    // Strings and regex
    .Match(CreateStringParser('"'), SewTokenType.@string)
    .Match(CreateStringParser('\''), SewTokenType.@string)
    .Match(CreateStringParser('/'), SewTokenType.regex)
    // Numbers
    .Match(SSpan.Regex("\\d+[\\.\\d*]?"), SewTokenType.number)
    // Symbols
    .Match(SIdentifier.CStyle, SewTokenType.symbol)
    .Build();

  public static CriusNyx.Results.Result<TokenList<SewTokenType>, SewLangException> Tokenize(
    string source
  )
  {
    var result = Tokenizer.TryTokenize(source);
    if (result.HasValue)
    {
      return result.Value;
    }
    else
    {
      return new SewLangException([
        new SewError(
          new(result.ErrorPosition.Absolute, 1),
          result.ErrorMessage ?? "Failed to tokenize"
        ),
      ]);
    }
  }
}
