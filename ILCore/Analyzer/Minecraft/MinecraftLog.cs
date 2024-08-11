using System.Text;

namespace ILCore.Analyzer.Minecraft;

public class MinecraftLog
{
    public MinecraftLogType Type { get; set; }
    public string Message { get; set; }
    public string Time { get; set; }
    public string LogSource { get; set; }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append(string.IsNullOrEmpty(Time) ? "" : $"{Time} > ");
        builder.Append(string.IsNullOrEmpty(LogSource) ? "" : $"{LogSource} > ");
        builder.Append(Type != MinecraftLogType.Unknown ? $"{Type} > " : "");
        builder.Append(Message);
        return  builder.ToString();
    }
}

public enum MinecraftLogType
{
    Unknown,
    Info,
    Warn,
    Error,
    Debug,
    Fatal,
}