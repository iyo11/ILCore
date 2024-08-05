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
        if (!string.IsNullOrEmpty(Time))
        {
            builder.Append(Time);
            builder.Append(" > ");
        }

        if (Type != MinecraftLogType.Unknown)
        {
            builder.Append(Type);
            builder.Append(" > ");
        }

        if (!string.IsNullOrEmpty(LogSource))
        {
            builder.Append(LogSource);
            builder.Append(" > ");
        }
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