using System.Text;
using System.Text.Json.Serialization;
using CriusNyx.Util;

namespace Sew;

public class SewOverload(string description, IEnumerable<SewArg> args)
{
  public string Description => description;
  public IEnumerable<SewArg> Args => args;

  public SewOverload(string description, params SewArg[] args)
    : this(description, args as IEnumerable<SewArg>) { }

  public override string ToString()
  {
    return $"({Args.Select(x => x.ToString()!).StringJoin(", ")})";
  }
}

public class SewArg(
  string names,
  string type,
  string? defaultValue = null,
  string? description = null
)
{
  public string Name => names;
  public virtual string TypeName => type;
  public string? DefaultValue => defaultValue;
  public string? Description => description;

  public override string ToString()
  {
    StringBuilder builder = new StringBuilder();
    builder.Append($"{Name}: {TypeName}");
    if (DefaultValue != null)
    {
      builder.Append($" = {DefaultValue}");
    }
    if (Description != null)
    {
      builder.Append($" // {Description}");
    }
    return builder.ToString();
  }
}

public class FilterArg(string name, string? defaultValue = null, string? description = null)
  : SewArg(name, "filter", defaultValue, description) { }

public class TransformerArg(string name, string? defaultValue = null, string? description = null)
  : SewArg(name, "transformer", defaultValue, description) { }

public class StringArg(string name, string? defaultValue = null, string? description = null)
  : SewArg(name, "string", defaultValue, description) { }

public class FlagArg(string name, string defaultValue = "false", string? description = null)
  : SewArg(name, "flag", defaultValue, description) { }

public class NumArg(string name, string? defaultValue = null, string? description = null)
  : SewArg(name, "number", defaultValue, description) { }

public class SewMethod(
  IEnumerable<string> names,
  string returns,
  IEnumerable<SewOverload> overloads,
  IEnumerable<SewArg> optionalArgs,
  HasTransformer impl
)
{
  public IEnumerable<string> Names => names;
  public string Returns => returns;
  public IEnumerable<SewOverload> Overloads => overloads;
  public IEnumerable<SewArg> OptionalArgs => optionalArgs;

  [JsonIgnore]
  public HasTransformer Impl => impl;

  public override string ToString()
  {
    return $@"# {Names.StringJoin(", ")}
    
  Returns: {Returns}
   
  Overloads:
{Overloads.Select((x) => x.ToString()!).StringJoin("\n").Indent("    ")}

  Flags:
{OptionalArgs.Select((x) => x.ToString()!).StringJoin("\n").Indent("    ")}";
  }
}
