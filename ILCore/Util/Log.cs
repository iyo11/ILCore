using System.Runtime.CompilerServices;

namespace ILCore.Util;

internal enum LogLevel
{
    Info,
    Debug,
    Warn,
    Error,
    Fatal
}

internal class LogMessage
{
    public LogLevel Level { get; set; }
    public DateTime Timestamp { get; set; }
    public string Message { get; set; }
    public string CallerName { get; set; }
    public string Path { get; set; }
    public int Line { get; set; }
}

public static class Log
{
    private static void WriteLogConsole(LogMessage logMessage)
    {
        var oldColor = Console.ForegroundColor;
        Console.ForegroundColor = logMessage.Level switch
        {
            LogLevel.Debug => ConsoleColor.DarkCyan,
            LogLevel.Info => ConsoleColor.White,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Warn => ConsoleColor.DarkMagenta,
            LogLevel.Fatal => ConsoleColor.DarkRed,
            _ => Console.ForegroundColor
        };
        var msg =
            $"[{logMessage.Timestamp:HH:mm:ss}]"
            + $"[{logMessage.Level}] -> "
            + $"{logMessage.Path} > {logMessage.CallerName}() > in line [{logMessage.Line,3}]: "
            + logMessage.Message;
        Console.WriteLine(msg);
        Console.ForegroundColor = oldColor;
    }

    public static void Info(string message,
        [CallerMemberName] string callerName = null,
        [CallerFilePath] string path = null,
        [CallerLineNumber] int line = 0)
    {
        path = Path.GetFileName(path);
        var logMessage = new LogMessage
        {
            Message = message,
            CallerName = callerName,
            Path = path,
            Line = line,
            Level = LogLevel.Info,
            Timestamp = DateTime.UtcNow
        };
        WriteLogConsole(logMessage);
    }

    public static void Debug(string message,
        [CallerMemberName] string callerName = null,
        [CallerFilePath] string path = null,
        [CallerLineNumber] int line = 0)
    {
        path = Path.GetFileName(path);
        var logMessage = new LogMessage
        {
            Message = message,
            CallerName = callerName,
            Path = path,
            Line = line,
            Level = LogLevel.Debug,
            Timestamp = DateTime.UtcNow
        };
        WriteLogConsole(logMessage);
    }

    public static void Warn(string message,
        [CallerMemberName] string callerName = null,
        [CallerFilePath] string path = null,
        [CallerLineNumber] int line = 0)
    {
        path = Path.GetFileName(path);
        var logMessage = new LogMessage
        {
            Message = message,
            CallerName = callerName,
            Path = path,
            Line = line,
            Level = LogLevel.Warn,
            Timestamp = DateTime.UtcNow
        };
        WriteLogConsole(logMessage);
    }

    public static void Error(string message,
        [CallerMemberName] string callerName = null,
        [CallerFilePath] string path = null,
        [CallerLineNumber] int line = 0)
    {
        path = Path.GetFileName(path);
        var logMessage = new LogMessage
        {
            Message = message,
            CallerName = callerName,
            Path = path,
            Line = line,
            Level = LogLevel.Error,
            Timestamp = DateTime.UtcNow
        };
        WriteLogConsole(logMessage);
    }

    public static void Fatal(string message,
        [CallerMemberName] string callerName = null,
        [CallerFilePath] string path = null,
        [CallerLineNumber] int line = 0)
    {
        path = Path.GetFileName(path);
        var logMessage = new LogMessage
        {
            Message = message,
            CallerName = callerName,
            Path = path,
            Line = line,
            Level = LogLevel.Fatal,
            Timestamp = DateTime.UtcNow
        };
        WriteLogConsole(logMessage);
    }
}