using CriusNyx.Util;
using Pastel;

namespace Sew.CLI;

public class AppException
{
  public IEnumerable<AppError> Errors { get; private set; }

  public AppException(IEnumerable<AppError> errors)
  {
    Errors = errors;
  }

  public AppException(params AppError[] errors)
  {
    Errors = errors;
  }

  public AppException Combine(AppException other)
  {
    return new AppException(Errors.Concat(other.Errors));
  }

  public static AppException Combine(AppException self, AppException other)
  {
    return self.Combine(other);
  }

  public string FormatError(bool chalk = false)
  {
    return Errors.Select(error => error.FormatError(chalk)).StringJoin("\n");
  }
}

public abstract class AppError
{
  public abstract string Code { get; }
  public abstract string Message { get; }

  public virtual string FormatError(bool chalk = false)
  {
    if (chalk)
    {
      return $"{(Code + ":").Pastel(ConsoleColor.Red).Bold()} {Message}";
    }
    return $"{Code}: {Message}";
  }
}

public class CommonError(string code, string message) : AppError
{
  public override string Code => code;
  public override string Message => message;
}

public class CLIParserError(CommandLine.Error inner) : AppError
{
  public CommandLine.Error Inner => inner;

  public override string Code => Inner.Tag.ToString();
  public override string Message => Inner.ToString() ?? "";
}
