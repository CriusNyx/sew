using CriusNyx.Results;

namespace Sew.CLI;

public class State
{
  public Sharpie.Event? currentSharpieEvent { get; private set; }
  public ReplEvent? currentAppEvent { get; private set; }
  public string textInput { get; private set; } = "";
  public Result<SewLangOutput, SewLangException>? lastSewResult { get; private set; } = null;
  public string sewInput { get; private set; } = "";
  public int inputPosition { get; private set; } = 0;
  public Span? selection { get; private set; } = null;
  public bool ShowDebugLine { get; private set; } = false;
  public bool ShowAST { get; private set; } = false;
  public int scroll { get; private set; } = 0;

  public void ProcessEvent(ReplEvent appEvent, Sharpie.Event? currentSharpieEvent)
  {
    this.currentSharpieEvent = currentSharpieEvent;
    this.currentAppEvent = appEvent;

    if (appEvent is { type: ReplEventType.TextInput })
    {
      Type(appEvent.keystroke);
    }
    else if (appEvent is { type: ReplEventType.Backspace })
    {
      Backspace();
    }
    else if (appEvent is { type: ReplEventType.BackspaceWord })
    {
      BackspaceWord();
    }
    else if (appEvent is { type: ReplEventType.Delete })
    {
      Delete();
    }
    else if (appEvent is { type: ReplEventType.Navigate })
    {
      ProcessNavEvent(appEvent);
    }
    else if (appEvent is { type: ReplEventType.ToggleDebug })
    {
      ShowDebugLine = !ShowDebugLine;
    }
    else if (appEvent is { type: ReplEventType.ToggleAST })
    {
      ShowAST = !ShowAST;
    }
    else if (appEvent is { type: ReplEventType.Scroll })
    {
      Scroll(appEvent.scrollAmount);
    }
    else if (
      appEvent is
      {
        type: ReplEventType.TextProcessed,
        sewResult: Result<SewLangOutput, SewLangException> result
      }
    )
    {
      lastSewResult = result;
    }
  }

  private void ProcessNavEvent(ReplEvent appEvent)
  {
    switch (appEvent.navigationType)
    {
      case NavigationType.LeftOne:
        MoveCursor(-1);
        break;
      case NavigationType.RightOne:
        MoveCursor(1);
        break;
      case NavigationType.LeftWord:
        JumpLeft();
        break;
      case NavigationType.RightWord:
        JumpRight();
        break;
      case NavigationType.Home:
        CursorHome();
        break;

      case NavigationType.End:
        CursorEnd();
        break;
    }
  }

  private void Type(char character)
  {
    sewInput = sewInput.Insert(inputPosition, character.ToString());
    inputPosition++;
  }

  private void Backspace()
  {
    if (sewInput.Length > 0 && inputPosition > 0)
    {
      sewInput = sewInput.Remove(inputPosition - 1, 1);
      inputPosition--;
    }
  }

  private void BackspaceWord()
  {
    var startPosition = JumpLeftIndex();
    sewInput = sewInput.Remove(startPosition, inputPosition - startPosition);
    inputPosition = startPosition;
  }

  private void Delete()
  {
    if (inputPosition < sewInput.Length)
    {
      sewInput = sewInput.Remove(inputPosition, 1);
    }
  }

  private void MoveCursor(int distance)
  {
    inputPosition += distance;
    inputPosition = Math.Clamp(inputPosition, 0, sewInput.Length);
  }

  private void CursorHome()
  {
    inputPosition = 0;
  }

  private void CursorEnd()
  {
    inputPosition = sewInput.Length;
  }

  public void Scroll(int amount)
  {
    scroll += amount;
  }

  private int JumpLeftIndex()
  {
    var index = inputPosition - 1;

    while (index >= 0 && !char.IsAsciiLetterOrDigit(sewInput[index]))
    {
      index--;
    }

    while (index >= 0 && char.IsAsciiLetterOrDigit(sewInput[index]))
    {
      index--;
    }

    index++;
    return index;
  }

  private void JumpLeft()
  {
    inputPosition = JumpLeftIndex();
  }

  private int JumpRightIndex()
  {
    var index = inputPosition;
    while (index >= 0 && index < sewInput.Length && char.IsAsciiLetterOrDigit(sewInput[index]))
    {
      index++;
    }

    while (index >= 0 && index < sewInput.Length && !char.IsAsciiLetterOrDigit(sewInput[index]))
    {
      index++;
    }

    return index;
  }

  private void JumpRight()
  {
    inputPosition = JumpRightIndex();
  }

  public static State From(ReplConfig config)
  {
    return new State() { textInput = config.Input ?? "" };
  }
}
