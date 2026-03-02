using CriusNyx.Util;
using Sew;
using Superpower;
using static Sew.SewBuilder;

namespace tests;

public class ParserTests
{
  static object[] ParserCases =>
    [
      // Identifier
      new object[] { "symbol", Ident("symbol") },
      // Invert
      new object[] { "!symbol", Inv(Ident("symbol")) },
      // Invocation
      new object[] { "lines()", Invoke("lines") },
      new object[] { "lines(append)", Invoke("lines", ValArg("append")) },
      new object[]
      {
        "lines(append, joiner: '\\n')",
        Invoke("lines", ValArg("append"), Named("joiner", StrLit("'\\n'"))),
      },
      new object[] { "tag('li', :nl)", Invoke("tag", ValArg(StrLit("'li'")), FlagArg("nl")) },
      // Num Lit
      new object[] { "0", Num("0") },
      // Pipe
      new object[] { "split | join", Pipe(Ident("split"), Ident("join")) },
      new object[] { "split | some | join", Pipe(Ident("split"), Ident("some"), Ident("join")) },
      // Regex Lit
      new object[] { "/.*/", RegexLit("/.*/") },
      // Regex Transformer
      new object[] { "/.*/ '$1'", RegexLit("/.*/", "'$1'") },
      // String Lit
      new object[] { "''", StrLit("''") },
      new object[] { "\"\"", StrLit("\"\"") },
      // Unit
      new object[] { "(l)", Unit(Ident("l")) },
      new object[]
      {
        "(split | some | join)",
        Unit(Pipe(Ident("split"), Ident("some"), Ident("join"))),
      },
    ];

  [Test, TestCaseSource(nameof(ParserCases))]
  public void CanParseSource(string source, ASTNode expected)
  {
    var actual = SewParser
      .SewProgram.Parse(SewTokenizer.Tokenize(source).Unwrap())!
      .AsNotNull<SewProgram>();
    List<ASTEquivalentError> errors = new List<ASTEquivalentError>();
    try
    {
      Assert.That(actual.Body!.ASTEquivalent(expected, errors));
    }
    catch
    {
      Console.WriteLine("Actual");
      Console.WriteLine(actual.Body!.Debug());
      Console.WriteLine("");
      Console.WriteLine("Expected");
      Console.WriteLine(expected.Debug());
      Console.WriteLine("");
      Console.WriteLine("Errors");
      foreach (var error in errors)
      {
        Console.WriteLine(error.Message);
        Console.WriteLine(error.Self.Debug());
        Console.WriteLine(error.Other.Debug());
      }
    }
  }
}
