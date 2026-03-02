// See https://aka.ms/new-console-template for more information
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Text.Json.Serialization;
using CriusNyx.Util;
using Sew;
using Sew.Examples;

return 0;

public class JSIResult
{
  public bool hasValue { get; set; }
  public string[] value { get; set; }
  public string error { get; set; }

  public static JSIResult Ok(string[] value)
  {
    return new JSIResult { hasValue = true, value = value };
  }

  public static JSIResult Err(string error)
  {
    return new JSIResult { hasValue = false, error = error };
  }
}

[JsonSourceGenerationOptions(
  WriteIndented = true,
  Converters = [typeof(JsonStringEnumConverter<SemanticType>)]
)]
[JsonSerializable(typeof(JSIResult))]
[JsonSerializable(typeof(SewMethod[]))]
[JsonSerializable(typeof(SewExample[]))]
[JsonSerializable(typeof(JSISemanticToken[]))]
internal partial class JSIGenerationContext : JsonSerializerContext { }

public class JSISemanticToken(int start, int length, SemanticType semanticType)
{
  public int Start => start;
  public int Length => length;
  public SemanticType SemanticType => semanticType;

  public static JSISemanticToken From(SemanticToken token)
  {
    var span = token.Token.Span;
    return new JSISemanticToken(span.Position.Absolute, span.Length, token.Type);
  }
}

partial class SewJSI
{
  [JSExport]
  internal static string Process(string input, string sewProgram)
  {
    var result = SewLang
      .Eval(input, sewProgram)
      .Map(x => JSIResult.Ok(x.Output.ToArray()))
      .UnwrapOrElse((err) => JSIResult.Err(err.Debug()));

    return JsonSerializer.Serialize(result, typeof(JSIResult), JSIGenerationContext.Default);
  }

  [JSExport]
  internal static string Methods()
  {
    return JsonSerializer.Serialize(
      SewRuntime.Methods,
      typeof(SewMethod[]),
      JSIGenerationContext.Default
    );
  }

  [JSExport]
  internal static string MethodExamples()
  {
    return JsonSerializer.Serialize(
      SewExamples.GenerateExamples(),
      typeof(SewExample[]),
      JSIGenerationContext.Default
    );
  }

  [JSExport]
  internal static string AnalyzeSemantics(string sewProgram)
  {
    var result = SewLang
      .Tokenize(sewProgram)
      .Map(tokens => tokens.Select(SemanticToken.From).Select(JSISemanticToken.From).ToList())
      .UnwrapOr(new List<JSISemanticToken>());

    for (var i = 0; i < result.Count; i++)
    {
      var value = result[i];
      var next = result.Safe(i + 1);
      var end = value.Start + value.Length;
      if (next == null)
      {
        if (end != sewProgram.Length)
        {
          result.Add(new JSISemanticToken(end, sewProgram.Length - end, SemanticType.none));
        }
      }
      else if (end != next.Start)
      {
        result.Insert(i + 1, new JSISemanticToken(end, next.Start - end, SemanticType.none));
      }
    }

    return JsonSerializer.Serialize(
      result.ToArray(),
      typeof(JSISemanticToken[]),
      JSIGenerationContext.Default
    );
  }
}
