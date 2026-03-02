using System.Text.RegularExpressions;
using CriusNyx.Util;

namespace Sew;

public static class SewRuntime
{
  public static Transformer Lines(Closure closure)
  {
    var args = closure.Unwrap(["arg1", "arg2"]).GetEnumerator();
    var arg1 = args.Consume();
    var arg2 = args.Consume();

    var elseVar = closure.GetNamedValue<HasTransformer>("else");
    Transformer? elseTransformer = elseVar != null ? elseVar.Transform : null;

    if (arg1 is HasPredicate pred && arg2 is HasTransformer trans1)
    {
      return (source) =>
        source.Select(str =>
          str.Split("\n")
            .SelectMany(line =>
              ApplyWhere([line], pred.GetPredicate(), trans1.Transform, elseTransformer)
            )
            .StringJoin("\n")
        );
    }
    else if (arg1 is HasTransformer trans2)
    {
      return (source) =>
        source.Select(str =>
          str.Split("\n").SelectMany((str) => trans2.Transform([str])).StringJoin("\n")
        );
    }
    else
    {
      return (source) => source.SelectMany((str) => str.Split("\n"));
    }
  }

  public static Transformer Paragraphs(Closure closure)
  {
    IEnumerable<string> Splitter(string input)
    {
      var lines = input.Split("\n");
      List<string> output = new List<string>();
      List<string> block = new List<string>();

      void CloseBlock()
      {
        if (block.Count != 0)
        {
          output.Add(block.StringJoin("\n"));
        }
        block.Clear();
      }
      foreach (var line in lines)
      {
        if (string.IsNullOrEmpty(line))
        {
          CloseBlock();
        }
        else
        {
          block.Add(line);
        }
      }

      CloseBlock();
      return output;
    }

    var args = closure.Unwrap(["arg1", "arg2"]).GetEnumerator();
    var arg1 = args.Consume();
    var arg2 = args.Consume();

    var elseVar = closure.GetNamedValue<HasTransformer>("else");
    Transformer? elseTransformer = elseVar != null ? elseVar.Transform : null;

    if (arg1 is HasPredicate pred && arg2 is HasTransformer trans1)
    {
      return (source) =>
        source.Select(str =>
          Splitter(str)
            .SelectMany(line =>
              ApplyWhere([line], pred.GetPredicate(), trans1.Transform, elseTransformer)
            )
            .StringJoin("\n")
        );
    }
    else if (arg1 is HasTransformer trans2)
    {
      return (source) =>
        source.Select(str =>
          Splitter(str).SelectMany((str) => trans2.Transform([str])).StringJoin("\n\n")
        );
    }
    else
    {
      return (source) => source.SelectMany((str) => str.Split("\n\n"));
    }
  }

  public static Transformer Split(Closure closure)
  {
    var elements = closure.Unwrap("separator", "predicate", "transformer").GetEnumerator();
    var separator = elements.Consume("\n");
    var arg1 = elements.Consume();
    var arg2 = elements.Consume();

    if (arg1 is HasPredicate hasPredicate && arg2 is HasTransformer hasTransformer)
    {
      var predicate = hasPredicate.GetPredicate();
      Transformer transformer = hasTransformer.Transform;
      return (source) =>
        source.Select(str =>
          ApplyWhere(str.Split(separator), predicate, transformer).StringJoin(separator)
        );
    }
    else if (arg1 is HasTransformer hasTransformer2)
    {
      return (source) =>
        source.Select(
          (str) => hasTransformer2.Transform(str.Split(separator)).StringJoin(separator)
        );
    }
    else
    {
      return (source) => source.SelectMany((str) => str.Split(separator));
    }
  }

  public static Transformer Join(Closure closure)
  {
    var elements = closure.Unwrap("separator").GetEnumerator();
    var separator = elements.Consume("\n");
    return (source) => [source.StringJoin(separator)];
  }

  public static Func<Closure, Transformer> CreateTrimTransformer(
    Func<string, char[], string> charTrimFunc
  )
  {
    return (closure) =>
    {
      var args = closure.Unwrap("trim").GetEnumerator();
      var trimString = args.Consume<string>() ?? "";

      return (values) => values.Select((x) => charTrimFunc(x, trimString.ToCharArray()));
    };
  }

  public static Transformer Quote(Closure closure)
  {
    var args = closure.Unwrap("quote").GetEnumerator();
    var quote = args.Consume<string>();
    var doubleQuote = closure.GetFlag("double", "d");
    var singleQuote = closure.GetFlag("single", "s");
    string quoteChar = "\"";
    if (singleQuote)
    {
      quoteChar = "'";
    }
    if (doubleQuote)
    {
      quoteChar = "\"";
    }
    if (quote != null)
    {
      quoteChar = quote;
    }

    return (values) => values.Select((str) => $"{quoteChar}{str}{quoteChar}");
  }

  public static Transformer Embed(Closure closure)
  {
    var args = closure.Unwrap("before", "after").GetEnumerator();
    var before = args.Consume("");
    var after = args.Consume(before);
    return CreateEmbed(before, after);
  }

  private static Transformer CreateEmbed(
    string before,
    string after,
    Func<string, string> trans = null!
  )
  {
    trans = trans ?? Function.Identity;
    return (values) => values.Select((str) => $"{before}{trans(str)}{after}");
  }

  public static Transformer Tag(Closure closure)
  {
    var args = closure.Unwrap("name", "props").GetEnumerator();
    var tag = args.Consume("");
    var props = args.Consume<string>();
    var newline = closure.GetFlag("newline", "newLine", "nl", "br");
    var indentAmountRaw = closure.GetNamedValue("indent");
    var indentAmount = indentAmountRaw?.As<decimal?>();

    if (newline && indentAmount == null)
    {
      indentAmount = 2;
    }

    Func<string, string> indentationFunc =
      indentAmount != null
        ? (str) => str.Indent("".PadLeft((int)indentAmount.Value))
        : Function.Identity;

    var open = $"<{tag}>";
    if (props != null)
    {
      open = $"<{tag} {props}>";
    }
    var close = $"</{tag}>";
    if (newline)
    {
      return CreateEmbed($"{open}\n", $"\n{close}", indentationFunc);
    }
    return CreateEmbed(open, close, indentationFunc);
  }

  public static Transformer Append(Closure closure)
  {
    var args = closure.Params<string>();
    return (values) => values.Select(str => $"{str}{args.StringJoin()}");
  }

  public static Transformer AppendStart(Closure closure)
  {
    var args = closure.Params<string>();
    return (values) => values.Select(str => $"{args.StringJoin()}{str}");
  }

  public static Transformer Indent(Closure closure)
  {
    decimal size = closure.GetNamedValue("size") as decimal? ?? 4;
    var args = closure.Unwrap("indentation").GetEnumerator();
    string indentation = "".PadLeft((int)size);
    var indentationArg = args.Consume();
    if (indentationArg is string str)
    {
      indentation = str;
    }
    if (indentationArg is decimal dec)
    {
      indentation = "".PadLeft((int)dec);
    }

    return (values) => values.Select((str) => str.Indent(indentation));
  }

  public static Transformer Replace(Closure closure)
  {
    var args = closure.Unwrap("filter", "replacement").GetEnumerator();
    var filter = args.Consume("");
    var replacement = args.Consume("");

    return (values) => values.Select(x => x.Replace(filter, replacement));
  }

  public static Transformer Where(Closure closure)
  {
    var args = closure.Unwrap("predicate", "transformer").GetEnumerator();
    var predicateContainer = args.Consume<HasPredicate>();
    var transformerContainer = args.Consume<HasTransformer>();

    var elseVar = closure.GetNamedValue<HasTransformer>("else");
    Transformer? elseTransformer = elseVar != null ? elseVar.Transform : null;

    var predicate = predicateContainer?.GetPredicate() ?? Function.Tautology;
    Transformer transformer =
      transformerContainer?.Transform(x => (Transformer)x.Transform) ?? Function.Identity;

    return (values) => ApplyWhere(values, predicate, transformer, elseTransformer);
  }

  private static IEnumerable<string> ApplyWhere(
    IEnumerable<string> values,
    Predicate predicate,
    Transformer transformer,
    Transformer? elseTransformer = null
  )
  {
    return values.SelectMany(
      (str) =>
      {
        if (predicate?.Invoke(str) ?? false)
        {
          return transformer?.Invoke([str]) ?? [str];
        }

        return elseTransformer?.Invoke([str]) ?? [str];
      }
    );
  }

  // Predicate functions
  public static Predicate Empty(Closure closure)
  {
    return string.IsNullOrEmpty;
  }

  public static Predicate NonEmpty(Closure closure)
  {
    return (str) => !string.IsNullOrEmpty(str);
  }

  public static Predicate Has(Closure closure)
  {
    // Unwrap args
    var has = closure.Params<object>().ToArray();
    var caseInsensitive = closure.GetFlag("insensitive", "lower");
    var all = closure.GetFlag("all", "every");

    Func<string, string> stringTransformer = caseInsensitive
      ? (string x) => x.ToLower()
      : Function.Identity<string>;

    // Create predicates
    var hasPredicates = has.Select(
        (x) =>
        {
          if (x is string str)
          {
            return new Predicate(
              (other) => stringTransformer(other).Contains(stringTransformer(str))
            );
          }
          return null;
        }
      )
      .WhereAs<Predicate>();

    if (all)
    {
      return (value) => hasPredicates.All((candidate) => candidate(value));
    }

    return (value) => hasPredicates.Any((candidate) => candidate(value));
  }

  public static PredicateFunc FromStringBasedPredicate(Func<string, string, bool> sourcePredicate)
  {
    return new PredicateFunc(
      (closure) =>
      {
        var values = closure.Unwrap("").GetEnumerator();
        var other = values.Consume("");
        return (str) => sourcePredicate(str, other);
      }
    );
  }

  public static PredicateFunc FromStringBasedPredicateInsensitive(
    Func<string, string, bool> sourcePredicate
  )
  {
    return new PredicateFunc(
      (closure) =>
      {
        var insensitive =
          closure.GetNamedValue("insensitive", "lower") is BooleanValue { Value: true };
        var values = closure.Unwrap("").GetEnumerator();
        var other = values.Consume("");
        if (insensitive)
        {
          return (str) => sourcePredicate(str.ToLower(), other.ToLower());
        }
        else
        {
          return (str) => sourcePredicate(str, other);
        }
      }
    );
  }

  public static Func<string, bool> Contains(Closure closure)
  {
    // No references?
    var args = closure.Unwrap("substring").GetEnumerator();
    var substring = args.Consume("");
    return (str) => str.Contains(substring);
  }

  public static object? DerefRuntimeValue(string identifier)
  {
    var methods = _MethodsByAlias.Value;
    var method = methods.Safe(identifier);
    return method?.Impl;
  }

  private static Thunk<SewMethod[]> _MethodsArr = new Thunk<SewMethod[]>(GenerateMethodsArr);

  public static SewMethod[] Methods => _MethodsArr.Value;

  private static Thunk<Dictionary<string, SewMethod>> _MethodsByAlias = new Thunk<
    Dictionary<string, SewMethod>
  >(_GenerateMethodsDictionary);

  private static SewMethod[] GenerateMethodsArr()
  {
    return
    [
      new SewMethod(
        ["lines", "l"],
        "transformer",
        [
          new("Split the input into lines returning a new list of the lines."),
          new("Apply the transformer to every line", new TransformerArg("transformer")),
          new(
            "Apply the transformer to every line that matches the filter.",
            new FilterArg("filter"),
            new TransformerArg("transformer")
          ),
        ],
        [],
        new FunctionTransformer(Lines)
      ),
      new SewMethod(
        ["paragraphs", "p"],
        "transformer",
        [
          new("Split the input into paragraphs, returning a new list of the paragraphs."),
          new(
            "Split the input into paragraphs. Apply the transformer to each paragraph, then join them back together.",
            new TransformerArg("transformer")
          ),
          new(
            "Split the input input paragraphs. Apply the transformer to each paragraph that matches the filter, then join them back together.",
            new FilterArg("filter"),
            new TransformerArg("transformer")
          ),
        ],
        [],
        new FunctionTransformer(Paragraphs)
      ),
      new SewMethod(
        ["split", "s"],
        "transformer",
        [
          new SewOverload(
            "Split the input anywhere where the separator is found.",
            new StringArg("separator", "\"\\n\"")
          ),
        ],
        [],
        new FunctionTransformer(Split)
      ),
      new SewMethod(
        ["join", "j"],
        "transformer",
        [
          new SewOverload(
            "Join the input together with the specified joiner.",
            new StringArg("joiner", "\"\\n\"")
          ),
        ],
        [],
        new FunctionTransformer(Join)
      ),
      new SewMethod(
        ["lower", "low"],
        "transformer",
        [new SewOverload("Convert the input into lowercase.")],
        [],
        FunctionTransformer.FromMap((x) => x.ToLower())
      ),
      new SewMethod(
        ["upper", "up"],
        "transformer",
        [new SewOverload("Convert the input into upper case.")],
        [],
        FunctionTransformer.FromMap((x) => x.ToUpper())
      ),
      new SewMethod(
        ["escape"],
        "transformer",
        [new SewOverload("Escape special characters in the input.")],
        [],
        FunctionTransformer.FromMap(x => Regex.Escape(x))
      ),
      new SewMethod(
        ["trim", "t"],
        "transformer",
        [
          new SewOverload(
            "Remove all leading and trailing characters provided. If no characters are provided removes whitespace.",
            new StringArg("trimCharacters", "null")
          ),
        ],
        [],
        new FunctionTransformer(CreateTrimTransformer((x, c) => x.Trim(c)))
      ),
      new SewMethod(
        ["trimL", "trimLeft", "trimStart", "tl", "ts"],
        "transformer",
        [
          new SewOverload(
            "Trim only the start of the string.",
            new StringArg("trimCharacters", "null")
          ),
        ],
        [],
        new FunctionTransformer(CreateTrimTransformer((x, c) => x.TrimStart(c)))
      ),
      new SewMethod(
        ["trimR", "trimRight", "trimEnd", "tr", "te"],
        "transformer",
        [
          new SewOverload(
            "Trim only the end of the string.",
            new SewArg("trimCharacters", "string", "null")
          ),
        ],
        [],
        new FunctionTransformer(CreateTrimTransformer((x, c) => x.TrimEnd(c)))
      ),
      new SewMethod(
        ["quote", "quoted"],
        "transformer",
        [new SewOverload("Wrap the string in quotes", new StringArg("quoteCharacter", "\"\""))],
        [
          new FlagArg("single", description: "If specified use single quote characters."),
          new FlagArg("double", description: "If specified use double quote characters."),
        ],
        new FunctionTransformer(Quote)
      ),
      new SewMethod(
        ["embed"],
        "transformer",
        [
          new SewOverload(
            "Embed the input between the start and end strings",
            new StringArg("start", "null"),
            new StringArg("end", "null")
          ),
        ],
        [],
        new FunctionTransformer(Embed)
      ),
      new SewMethod(
        ["tag"],
        "transformer",
        [new SewOverload("Wrap the input in an html tag", new SewArg("tag", "string", "null"))],
        [
          new FlagArg("newline", "false", "Move the input onto a new line with a 2 space indent."),
          new NumArg("indent", "null", "Specify the number of spaces to use to indent."),
        ],
        new FunctionTransformer(Tag)
      ),
      new SewMethod(
        ["append", "app", "concat", "cat"],
        "transformer",
        [
          new SewOverload(
            "Append the input to the end of the string.",
            new SewArg("other", "string", "null")
          ),
        ],
        [],
        new FunctionTransformer(Append)
      ),
      new SewMethod(
        ["appendL", "appendLeft", "appendStart", "appl", "catl"],
        "transformer",
        [new SewOverload("Append the input before the string.", new StringArg("other", "null"))],
        [],
        new FunctionTransformer(AppendStart)
      ),
      new SewMethod(
        ["indent"],
        "transformer",
        [new SewOverload("Indent source string by specified spaces.", new NumArg("indent", "4"))],
        [],
        new FunctionTransformer(Indent)
      ),
      new SewMethod(
        ["replace"],
        "transformer",
        [
          new SewOverload(
            "Replace any substring that matches the original with the replacement.",
            new StringArg("original"),
            new StringArg("replacement")
          ),
        ],
        [],
        new FunctionTransformer(Replace)
      ),
      new SewMethod(
        ["where", "w"],
        "transformer",
        [
          new SewOverload(
            "Apply the transformer to any string that matches the filter.",
            new FilterArg("filter"),
            new TransformerArg("transformer")
          ),
        ],
        [],
        new FunctionTransformer(Where)
      ),
      new SewMethod(
        ["none", "empty"],
        "filter",
        [new SewOverload("Filter that matches any string that is empty.")],
        [],
        new PredicateFunc(Empty)
      ),
      new SewMethod(
        ["some", "nonEmpty"],
        "predicate",
        [new SewOverload("Filter that matches any string that is not empty.")],
        [],
        new PredicateFunc(NonEmpty)
      ),
      new SewMethod(
        ["has", "contains"],
        "predicate",
        [
          new SewOverload(
            "Filter that matches any string that contains the values provided",
            new SewArg("values", "...string", "[]")
          ),
        ],
        [new FlagArg("lower"), new FlagArg("all")],
        new PredicateFunc(Has)
      ),
      new SewMethod(
        ["starts", "startsWith"],
        "predicate",
        [
          new SewOverload(
            "Filter that matches any string that starts with the provided value.",
            new StringArg("value", "string", "null")
          ),
        ],
        [new FlagArg("lower")],
        FromStringBasedPredicateInsensitive((x, y) => x.StartsWith(y))
      ),
      new SewMethod(
        ["ends", "endsWith"],
        "predicate",
        [
          new SewOverload(
            "Filter that matches any string that ends with the provided value.",
            new SewArg("value", "string", "null")
          ),
        ],
        [new FlagArg("lower")],
        FromStringBasedPredicateInsensitive((x, y) => x.EndsWith(y))
      ),
      new SewMethod(
        ["eq", "equals"],
        "predicate",
        [
          new SewOverload(
            "Filter that matches any string that matches this exactly.",
            new SewArg("value", "string", "null")
          ),
        ],
        [new SewArg("lower", "boolean", "false")],
        FromStringBasedPredicateInsensitive((x, y) => x == y)
      ),
    ];
  }

  private static Dictionary<string, SewMethod> _GenerateMethodsDictionary()
  {
    Dictionary<string, SewMethod> dictionary = new Dictionary<string, SewMethod>();

    foreach (var method in Methods)
    {
      foreach (var name in method.Names)
      {
        dictionary.Add(name, method);
      }
    }

    return dictionary;
  }
}
