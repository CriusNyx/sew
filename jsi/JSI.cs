// See https://aka.ms/new-console-template for more information
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

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(JSIResult))]
[JsonSerializable(typeof(SewMethod[]))]
[JsonSerializable(typeof(SewExample[]))]
internal partial class JSIGenerationContext : JsonSerializerContext { }

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
}
