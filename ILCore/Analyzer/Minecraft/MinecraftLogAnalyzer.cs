using System.Text.RegularExpressions;
using ILCore.Util;

namespace ILCore.Analyzer.Minecraft;

public partial class MinecraftLogAnalyzer
{

    private static readonly Regex LogPattern = Regexs.LogRegex();
    private static readonly string[] TypeArray = ["INFO", "WARN", "FATAL", "ERROR" , "DEBUG" , "UNKNOWN"];

    public async Task<MinecraftLog> Analyze(string log)
    {
        
        return await Task.Run(() =>
        {
            if (string.IsNullOrEmpty(log))
                return new MinecraftLog()
                {
                    Message = Emoticon.GetRandomEmoticon(),
                };
            var match = LogPattern.Match(log);
            if (!match.Success)
            {
                if (log.Contains("Exception"))
                {
                    return new MinecraftLog()
                    {
                        Message = log,
                        Type = MinecraftLogType.Error,
                        Time = $"[{DateTime.Now:HH:mm:ss}]",
                        LogSource = "[Exception]",
                    };
                }
                return new MinecraftLog()
                {
                    Message = log,
                };
            }

            var time = match.Groups[1].Value;
            var source = match.Groups[2].Value;
            var message = match.Groups[3].Value;
            var type = Enum.Parse<MinecraftLogType>(
                (TypeArray)
                .FirstOrDefault(x => source.Contains(x)) ?? "Unknown",
                true);


            var minecraftLog = new MinecraftLog
            {
                Time = time,
                LogSource = source,
                Message = message,
                Type = type
            };
            return minecraftLog;
        });
    }


}