using CriusNyx.Results;
using Sharpie;

namespace Sew.CLI;

/// <summary>
/// Event type for Repl event.
/// </summary>
public enum ReplEventType
{
  Exit,
  WriteToFile,
  WriteToClipboard,
  TextInput,
  Backspace,
  BackspaceWord,
  Delete,
  Scroll,
  Navigate,
  ToggleDebug,
  ToggleAST,
  Resize,
  Unknown,
  TextProcessed,
}

/// <summary>
/// Event subtype for navigation event.
/// </summary>
public enum NavigationType
{
  LeftOne,
  LeftWord,
  RightOne,
  RightWord,
  Home,
  End,

  None,
}

/// <summary>
/// An event caused by a keystroke or background data processing.
/// </summary>
public struct ReplEvent
{
  public ReplEventType type;
  public NavigationType navigationType;
  public char keystroke;
  public bool selectHeld;
  public int scrollAmount = 0;
  public Result<SewLangOutput, SewLangException>? sewResult;

  public ReplEvent(
    ReplEventType type,
    NavigationType navType = NavigationType.None,
    char keystroke = '\0',
    bool selectHeld = false
  )
  {
    this.type = type;
    navigationType = navType;
    this.keystroke = keystroke;
    this.selectHeld = selectHeld;
    sewResult = null!;
  }

  public override string ToString()
  {
    return $"AppEvent {{type: {type}, keystroke: {keystroke}}} navType: {navigationType} selectHeld: {selectHeld}";
  }

  /// <summary>
  /// Create a new navigation event.
  /// </summary>
  /// <param name="navType"></param>
  /// <returns></returns>
  public static ReplEvent NavEvent(NavigationType navType)
  {
    return new ReplEvent(ReplEventType.Navigate, navType);
  }

  /// <summary>
  /// Create a new sharpie event.
  /// </summary>
  /// <param name="event"></param>
  /// <returns></returns>
  public static ReplEvent FromSharpieEvent(Event @event)
  {
    // Resize

    if (@event is TerminalResizeEvent)
    {
      return new(ReplEventType.Resize);
    }

    if (@event is KeyEvent keyEvent)
    {
      // Exit
      if (
        keyEvent is
        { Key: Key.Character, Char.IsAscii: true, Char.Value: 'C', Modifiers: ModifierKey.Ctrl }
      )
      {
        return new(ReplEventType.Exit);
      }

      // Copy Exit
      if (
        keyEvent is
        { Key: Key.Character, Char.IsAscii: true, Char.Value: 'X', Modifiers: ModifierKey.Ctrl }
      )
      {
        return new ReplEvent(ReplEventType.WriteToClipboard);
      }

      // Letters
      if (keyEvent is { Key: Key.Character, Char.IsAscii: true, Modifiers: ModifierKey.None })
      {
        return new(ReplEventType.TextInput, keystroke: (char)keyEvent.Char.Value);
      }
      if (
        keyEvent is
        { Key: Key.Character, Char.IsAscii: true, Modifiers: ModifierKey.Shift } shiftEvent
      )
      {
        return new(ReplEventType.TextInput, keystroke: char.ToUpper((char)shiftEvent.Char.Value));
      }

      // Backspace
      if (keyEvent is { Key: Key.Backspace, Modifiers: ModifierKey.None })
      {
        return new(ReplEventType.Backspace);
      }
      if (keyEvent is { Key: Key.Backspace, Modifiers: ModifierKey.Ctrl })
      {
        return new(ReplEventType.BackspaceWord);
      }

      // Control + Backspace maps to CTRL-W for some reason.
      if (
        keyEvent is
        { Key: Key.Character, Char.IsAscii: true, Char.Value: 'W', Modifiers: ModifierKey.Ctrl }
      )
      {
        return new(ReplEventType.BackspaceWord);
      }

      // Delete
      if (keyEvent is { Key: Key.Delete })
      {
        return new(ReplEventType.Delete);
      }

      // Arrows
      if (keyEvent is { Key: Key.KeypadLeft })
      {
        if (keyEvent.Modifiers == ModifierKey.None)
        {
          return NavEvent(NavigationType.LeftOne);
        }
        if (keyEvent.Modifiers == ModifierKey.Ctrl)
        {
          return NavEvent(NavigationType.LeftWord);
        }
      }
      if (keyEvent is { Key: Key.KeypadRight })
      {
        if (keyEvent.Modifiers == ModifierKey.None)
        {
          return NavEvent(NavigationType.RightOne);
        }
        if (keyEvent.Modifiers == ModifierKey.Ctrl)
        {
          return NavEvent(NavigationType.RightWord);
        }
      }

      // CTRL-Left is the same as SHIFT-ALT-KeypadPageUp
      if (keyEvent is { Key: Key.KeypadPageUp, Modifiers: ModifierKey.Shift | ModifierKey.Alt })
      {
        return NavEvent(NavigationType.LeftWord);
      }
      // CTRL-Right is ȹ for some reason?
      // This is probably conflicting with some other kind of input. Investigate more.
      // Maybe there is some kind of input framework that can handle this better.
      if (keyEvent is { Key: Key.Unknown })
      {
        if (keyEvent.Name == "ȹ")
        {
          return NavEvent(NavigationType.RightWord);
        }
      }

      // Scroll
      if (keyEvent is { Key: Key.KeypadUp })
      {
        return new ReplEvent { type = ReplEventType.Scroll, scrollAmount = -1 };
      }
      if (keyEvent is { Key: Key.KeypadDown })
      {
        return new ReplEvent { type = ReplEventType.Scroll, scrollAmount = 1 };
      }
      if (keyEvent is { Key: Key.KeypadPageUp })
      {
        return new ReplEvent { type = ReplEventType.Scroll, scrollAmount = -10 };
      }
      if (keyEvent is { Key: Key.KeypadPageDown })
      {
        return new ReplEvent { type = ReplEventType.Scroll, scrollAmount = 10 };
      }

      // Home End
      if (keyEvent is { Key: Key.KeypadHome })
      {
        return NavEvent(NavigationType.Home);
      }
      if (keyEvent is { Key: Key.KeypadEnd })
      {
        return NavEvent(NavigationType.End);
      }

      // Debug
      if (
        keyEvent is
        { Key: Key.Character, Char.IsAscii: true, Char.Value: 'D', Modifiers: ModifierKey.Ctrl }
      )
      {
        return new(ReplEventType.ToggleDebug);
      }

      // AST
      if (
        keyEvent is
        { Key: Key.Character, Char.IsAscii: true, Char.Value: 'A', Modifiers: ModifierKey.Ctrl }
      )
      {
        return new(ReplEventType.ToggleAST);
      }
    }

    return new ReplEvent(ReplEventType.Unknown);
  }

  public static ReplEvent ProcessedTextEvent(Result<SewLangOutput, SewLangException> result)
  {
    return new ReplEvent(ReplEventType.TextProcessed) { sewResult = result };
  }
}
