using System.Drawing;
using CriusNyx.Util;
using Sharpie;
using Sharpie.Abstractions;

namespace Sew.CLI;

public class ReplInterface(Repl repl, Terminal terminal)
{
  public State state => repl.state;

  ColorMixture defaultColor;
  ColorMixture invertedColor;
  ColorMixture selectionColor;
  ColorMixture lineNumberColor;
  ColorMixture cursorColor;
  ColorMixture identifierColor;
  ColorMixture keywordColor;
  ColorMixture regexColor;
  ColorMixture stringColor;
  ColorMixture methodColor;

  private void Initialize()
  {
    InitializeColors();
  }

  private void InitializeColors()
  {
    defaultColor = terminal.Colors.MixColors(StandardColor.White, StandardColor.Black);
    invertedColor = terminal.Colors.MixColors(StandardColor.Black, StandardColor.White);
    selectionColor = terminal.Colors.MixColors(StandardColor.Black, StandardColor.Green);
    cursorColor = terminal.Colors.MixColors(StandardColor.Black, StandardColor.White);
    identifierColor = terminal.Colors.MixColors(StandardColor.Cyan, StandardColor.Default);
    keywordColor = terminal.Colors.MixColors(StandardColor.Blue, StandardColor.Default);
    regexColor = terminal.Colors.MixColors(StandardColor.Red, StandardColor.Default);
    stringColor = terminal.Colors.MixColors(StandardColor.Green, StandardColor.Default);
    methodColor = terminal.Colors.MixColors(StandardColor.Yellow, StandardColor.Default);
    lineNumberColor = terminal.Colors.MixColors(StandardColor.Green, StandardColor.Default);
  }

  public void DrawInterface(Terminal terminal)
  {
    if (state.currentAppEvent?.type == ReplEventType.Resize)
    {
      terminal.Screen.Clear();
    }

    var (body, bottom) = terminal.Screen.View().SplitV(-5);

    var (_, inputLine, bottomView) = bottom.SplitV(1).Expand(x => x.SplitV(2));

    DrawBody(body);

    inputLine.Window().Touch(DrawInputLine);
    DrawStatusLine(bottomView);
    terminal.Screen.Refresh();
  }

  public void DrawSemanticToken(IWindow window, Point caret, SemanticToken token)
  {
    if (token.Type == SemanticType.none)
    {
      return;
    }
    var style = new Style { };
    switch (token.Type)
    {
      case SemanticType.identifier:
        style = new Style { ColorMixture = identifierColor };
        break;
      case SemanticType.keyword:
        style = new Style { ColorMixture = keywordColor };
        break;
      case SemanticType.method:
        style = new Style { ColorMixture = methodColor };
        break;
      case SemanticType.regexLit:
        style = new Style { ColorMixture = regexColor };
        break;
      case SemanticType.stringLit:
        style = new Style { ColorMixture = stringColor };
        break;
    }
    window.CaretLocation = new Point(caret.X + token.Token.Position.Absolute, caret.Y);
    window.WriteText(token.Token.ToStringValue(), style, false);
  }

  public void DrawBody(View view)
  {
    var (left, right) = view.SplitH(view.rectangle.Width / 2);

    DrawLeft(left);
    DrawRight(right);
  }

  public void DrawLeft(View view)
  {
    var window = view.Window();
    window.Clear();
    DrawBlocksWithLineNumbers(window, [state.textInput]);
  }

  public void DrawRight(View view)
  {
    var window = view.Window();
    window.Clear();

    if (state.lastSewResult.IsNone())
    {
      return;
    }
    var evaluationResult = state.lastSewResult.Unwrap();

    if (evaluationResult.IsErr())
    {
      var err = evaluationResult.Err().Unwrap();
      window.WriteText(err.Debug());
      return;
    }

    var evaluation = evaluationResult.Unwrap();

    if (state.ShowAST)
    {
      DrawTextWithScroll(window, Point.Empty, evaluation.AST.Debug());
      return;
    }

    try
    {
      DrawBlocksWithLineNumbers(window, evaluation.Output);
    }
    catch (Exception exception)
    {
      DrawTextWithScroll(window, Point.Empty, exception.ToString());
    }
  }

  public void DrawInputLine(IWindow window)
  {
    var caret = window.CaretLocation;
    window.WriteText(state.sewInput);
    var endOfInput = window.CaretLocation;

    var tokensResult = SewLang.Tokenize(state.sewInput);
    tokensResult.Inspect(
      (tokens) =>
        tokens
          .Select(SemanticToken.From)
          .Foreach(
            (semantic) =>
            {
              DrawSemanticToken(window, caret, semantic);
            }
          )
    );

    window.CaretLocation = endOfInput;

    if (state.codeInputCursorPosition >= 0 && state.codeInputCursorPosition < state.sewInput.Length)
    {
      var cursorChar = state.sewInput[state.codeInputCursorPosition];
      window.CaretLocation = new(state.codeInputCursorPosition, 0);
      window.WriteText(cursorChar.ToString(), new Style { ColorMixture = cursorColor });
    }
    else
    {
      window.WriteText(" ", new Style { ColorMixture = cursorColor });
    }
  }

  public void DrawStatusLine(View view)
  {
    view.Window().Clear();
    if (state.ShowDebugLine)
    {
      DrawDebugLine(view);
    }
    else
    {
      DrawHotkeys(view);
    }
  }

  public void DrawDebugLine(View view)
  {
    var window = view.Window();
    window.WriteText(state.currentSharpieEvent?.ToEventString() ?? "");
    window.WriteText("\n");
    window.WriteText(state.currentAppEvent?.Debug() ?? "");
  }

  public void DrawHotkeys(View view)
  {
    var window = view.Window();
    foreach (var hotkey in repl.hotkeys)
    {
      hotkey.Draw(repl, window, invertedColor);
    }
  }

  #region  Helpers
  public void DrawTextWithScroll(IWindow window, Point point, string source)
  {
    var lines = source.Split("\n");
    foreach (var (line, index) in lines.WithIndex())
    {
      var p = new Point(point.X, point.Y + index - state.scroll);
      DrawAtLocation(window, p, (win) => win.WriteText(line, false));
    }
  }

  public void DrawBlocksWithLineNumbers(IWindow window, IEnumerable<string> sources)
  {
    var cursorIndex = 0;
    foreach (var (str, measure) in sources.Zip(sources.Select(x => x.Measure())))
    {
      DrawStringWithLineNumbers(window, new Point(0, cursorIndex), str);
      cursorIndex += measure.Height + 1;
    }
  }

  public void DrawStringWithLineNumbers(IWindow window, Point cursor, string source)
  {
    var lines = source.Split("\n");
    foreach (var (line, index) in lines.WithIndex())
    {
      var p = new Point(cursor.X, cursor.Y + index - state.scroll);
      DrawAtLocation(
        window,
        p,
        (win) =>
        {
          win.WriteText($"{index + 1, 3}: ", new Style() { ColorMixture = lineNumberColor }, false);
          win.WriteText(line, false);
        }
      );
    }
  }

  public void DrawAtLocation(IWindow window, Point point, Action<IWindow> action)
  {
    if (window.Size.Contains(point))
    {
      window.CaretLocation = point;
      action(window);
    }
  }
  #endregion

  public static ReplInterface CreateAndInitialize(Repl repl, Terminal terminal)
  {
    return new ReplInterface(repl, terminal).Touch(x => x.Initialize());
  }
}
