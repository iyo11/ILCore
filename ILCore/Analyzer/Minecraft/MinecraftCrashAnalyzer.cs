using ILCore.Util;

namespace ILCore.Analyzer.Minecraft;

public class MinecraftCrashAnalyzer
{
    public IEnumerable<string> Analyze(IEnumerable<MinecraftLog> minecraftLogs)
    {
        foreach (var minecraftLog in minecraftLogs)
        {
            var message = AccurateAnalyze(minecraftLog.Message);
            if (!string.IsNullOrEmpty(message))
                yield return message;
            message = FuzzyAnalyze(minecraftLog.Message);
            if (!string.IsNullOrEmpty(message))
                yield return message;
            yield return  message;
        }
    }

    private string AccurateAnalyze(string message)
    {
        var  messages = message.Split(":");
        if (messages.Length < 2) return null;
        return messages[0] switch
        {
            "net.minecraftforge.fml.common.MissingModsException" => Language.GetValue("ForgeMissingMods",
                Regexes.RoundBrackets().Match(messages[1]).Groups[0].Value,
                Regexes.SquareBrackets().Match(messages[1]).Groups[0].Value),
            _ => messages[1] switch
            {
                " Registering texture" => Language.GetValue("RegisteringTexture"),
                _ => null
            }
        };
    }

    private string FuzzyAnalyze(string message)
    {
        return message switch
        {
            not null when message.Contains("java.lang.ClassNotFoundException:") => Language.GetValue(
                "ClassNotFoundException", message.Split(':')[1]),
            not null when message.Contains("java.lang.ClassCastException: class jdk.") => Language.GetValue(
                "ClassCastException"),    
            not null when message.Contains("java.lang.OutOfMemoryError:") => Language.GetValue(
                "OutOfMemoryError", message.Split(':')[1]),
            _ => null
        };
    }
}