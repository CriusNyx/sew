using CriusNyx.Util;
using Superpower.Model;

namespace Sew;

public class RegexTransformer(RegexLit regexLit, StringLit stringLit) : ASTNode
{
  public RegexLit RegexLit => regexLit;
  public StringLit StringLit => stringLit;

  public override IEnumerable<(string, object)> EnumerateFields()
  {
    return [nameof(RegexLit).With(RegexLit), nameof(StringLit).With(StringLit)];
  }

  public override object? Evaluate(ExecutionContext context)
  {
    var regex = RegexLit.CreateRegex();
    var replacement = stringLit.Literal;
    return new FunctionTransformer(
      (_) => (source) => source.Select((str) => regex.Unwrap().Replace(str, replacement))
    );
  }

  public override IEnumerable<Token<SewTokenType>> GetSourceTokens()
  {
    return RegexLit.GetSourceTokens().Concat(StringLit.GetSourceTokens());
  }

  public override IEnumerable<SemanticToken> GetSemanticTokens()
  {
    return RegexLit.GetSemanticTokens().Concat(StringLit.GetSemanticTokens());
  }

  public override bool ASTEquivalent(ASTNode other, List<ASTEquivalentError> errors)
  {
    if (other is RegexTransformer trans)
    {
      return RegexLit.ASTEquivalent(trans.RegexLit, errors)
        && StringLit.ASTEquivalent(trans.StringLit, errors);
    }
    errors.Add(ASTEquivalentError.TypeError(this, other));
    return false;
  }
}
