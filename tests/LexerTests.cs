using Sew;

namespace tests;

public class LexerTests
{
  [TestCase("", new SewTokenType[] { })]
  [TestCase("  ", new SewTokenType[] { })]
  [TestCase("\t", new SewTokenType[] { })]
  [TestCase("\r", new SewTokenType[] { })]
  [TestCase("\n", new SewTokenType[] { })]
  [TestCase("\r\n", new SewTokenType[] { })]
  // Symbols
  [TestCase("!", new SewTokenType[] { SewTokenType.exclamation })]
  [TestCase("|", new SewTokenType[] { SewTokenType.pipe })]
  [TestCase("(", new SewTokenType[] { SewTokenType.openParen })]
  [TestCase(")", new SewTokenType[] { SewTokenType.closedParen })]
  [TestCase(",", new SewTokenType[] { SewTokenType.comma })]
  [TestCase(":", new SewTokenType[] { SewTokenType.colon })]
  // Strings
  [TestCase("\"string\"", new SewTokenType[] { SewTokenType.@string })]
  [TestCase("'string'", new SewTokenType[] { SewTokenType.@string })]
  // String escaped char
  [TestCase("\"\\\"\"", new SewTokenType[] { SewTokenType.@string })]
  [TestCase("'\\''", new SewTokenType[] { SewTokenType.@string })]
  // String mixed quotes
  [TestCase("\"string'\"", new SewTokenType[] { SewTokenType.@string })]
  // Regex
  [TestCase("'string\"'", new SewTokenType[] { SewTokenType.@string })]
  [TestCase("/regex/", new SewTokenType[] { SewTokenType.regex })]
  // Regex escaped
  [TestCase("/\\//", new SewTokenType[] { SewTokenType.regex })]
  // Numbers
  [TestCase("0", new SewTokenType[] { SewTokenType.number })]
  // Symbol
  [TestCase("hello", new SewTokenType[] { SewTokenType.symbol })]
  public void CanLexInput(string input, SewTokenType[] expected)
  {
    var actual = SewTokenizer.Tokenize(input).Unwrap().Select(x => x.Kind);
    Assert.That(actual, Is.EqualTo(expected));
  }
}
