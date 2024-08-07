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
            {
                yield return message;
            }
            else
            {
                message = FuzzyAnalyze(minecraftLog.Message);
                yield return message;
            }
        }
    }

    private string AccurateAnalyze(string minecraftLog)
    {
        var  logs = minecraftLog.Split(":");
        return logs[0] switch
        {
            "net.minecraftforge.fml.common.MissingModsException" => Language.GetValue("ForgeMissingMods",
                Regexs.RoundBrackets().Match(logs[1]).Groups[0].Value,
                Regexs.SquareBrackets().Match(logs[1]).Groups[0].Value),
            _ => null
        };
    }

    private string FuzzyAnalyze(string minecraftLog)
    {
        return minecraftLog.Contains("java.lang.ClassNotFoundException:") ? Language.GetValue("ClassNotFoundException",minecraftLog.Split(":")[1]) : null;
    }
}