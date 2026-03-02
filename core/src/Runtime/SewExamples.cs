namespace Sew.Examples;

public class SewExample(string methodName, string input, string code)
{
  public string MethodName => methodName;
  public string Input => input;
  public string Code => code;
}

public static class SewExamples
{
  public static SewExample[] GenerateExamples()
  {
    string trimExample = "  --title--  ";

    string markdownExample =
      @"# What is Sew?

Sew is an interactive command line tool 
that lets you write small string 
manipulation programs and see your 
results in real time. Like any good CLI 
tool it also supports input files and 
piped input.

I created sew because I wanted a tool 
that replaced the python scripts I was 
using the manipulate strings in my 
clipboard. I wanted to create something 
that had a nice grammar, was interactive 
in the command line, and had a user 
experience with hotkeys, similar to nano.

Let's look at some things it can do.";

    string csvExample =
      @"Read,The Three Body Problem,Lin Cixin
Read,The Buffalo Hunter Hunter,Stephen Graham Jones
Read,Homage to Catalonia,George Orwell
Read,The Glass Castle,Jeannette Walls
Read,The Invisible Life of Addie Larue,V.E. Schwab
Read,The Peace to End All Peace,David Fromkin
Read,Anxious People,Fredrik Backman
Read,Zuangzi,Zuangzi
Read,A Heartbreaking Work of Staggering Genius,Dave Eggers
Reading,The Dark Forest,Lin Cixin
Suggested,Everything is Tuberculosis,John Green";

    return
    [
      new SewExample("lines", markdownExample, "lines"),
      new SewExample("lines", markdownExample, "lines | some | join"),
      new SewExample("lines", markdownExample, "lines(some)"),
      new SewExample("lines", markdownExample, "lines(starts('#'), tag('h1'))"),
      new SewExample("paragraphs", markdownExample, "paragraphs"),
      new SewExample("paragraphs", markdownExample, "paragraphs | tag('p')"),
      new SewExample("paragraphs", markdownExample, "paragraphs(tag('p'))"),
      new SewExample(
        "paragraphs",
        markdownExample,
        "paragraphs(starts('#'), tag('h1'), else: tag('p', :nl))"
      ),
      new SewExample("split", csvExample, "lines | split(',')"),
      new SewExample("split", csvExample, "lines(split(',', quote))"),
      new SewExample("join", markdownExample, "lines | join"),
      new SewExample("join", markdownExample, "paragraphs | join"),
      new SewExample("join", markdownExample, "lines | join(',\\n')"),
      new SewExample("lower", markdownExample, "lower"),
      new SewExample("lower", markdownExample, "lines(where(starts('#'), lower))"),
      new SewExample("upper", markdownExample, "upper"),
      new SewExample("upper", markdownExample, "lines(where(starts('#'), upper))"),
      new SewExample("escape", csvExample, "escape"),
      new SewExample("escape", csvExample, "escape | quote"),
      new SewExample("trim", trimExample, "trim"),
      new SewExample("trim", trimExample, "trim | trim('-')"),
      new SewExample("trimL", trimExample, "trimL"),
      new SewExample("trimL", trimExample, "trimL | trimL('-')"),
      new SewExample("trimR", trimExample, "trimR"),
      new SewExample("trimR", trimExample, "trimR | trimR('-')"),
      new SewExample("quote", markdownExample, "quote"),
      new SewExample("quote", markdownExample, "lines(quote(:single))"),
      new SewExample("quote", markdownExample, "lines(quote(:double))"),
      new SewExample("quote", markdownExample, "lines(quote('/'))"),
      new SewExample("embed", markdownExample, "embed('[', ']')"),
      new SewExample("embed", markdownExample, "lines(quote | embed('[', ']'))"),
      new SewExample("tag", markdownExample, "tag('p')"),
      new SewExample("tag", markdownExample, "lines(tag('p'))"),
      new SewExample("tag", markdownExample, "lines(tag('p', :nl))"),
      new SewExample("tag", markdownExample, "lines(tag('p', :nl, indent:6))"),
      new SewExample("append", markdownExample, "append('secret')"),
      new SewExample("append", markdownExample, "lines(append('EOL'))"),
      new SewExample("appendL", markdownExample, "appendL('#Secret first title')"),
      new SewExample("appendL", markdownExample, "lines(appendL('line: '))"),
      new SewExample("replace", markdownExample, "replace('Sew', 'Sew Experimental')"),
      new SewExample(
        "replace",
        markdownExample,
        "replace('sew', 'sew experimental') | replace('Sew', 'Sew Experimental')"
      ),
      new SewExample("where", markdownExample, "lines(where(has('#'), tag('h1')))"),
      new SewExample(
        "where",
        markdownExample,
        "lines(where(has('#'), tag('h1'), else: tag('p'))))"
      ),
      new SewExample("none", markdownExample, "lines(none)"),
      new SewExample("none", markdownExample, "lines(where(none, quote))"),
      new SewExample("some", markdownExample, "lines(some)"),
      new SewExample("some", markdownExample, "lines(some | quote)"),
      new SewExample("some", markdownExample, "lines(where(some, quote))"),
      new SewExample("has", markdownExample, "lines(has('sew'))"),
      new SewExample("has", markdownExample, "lines(has('sew', :lower))"),
      new SewExample("has", markdownExample, "lines(where(has('sew'), upper))"),
      new SewExample("starts", markdownExample, "lines(where(starts('#'), upper))"),
      new SewExample("eq", markdownExample, "lines(!eq('# What is Sew?'))"),
    ];
  }
}
