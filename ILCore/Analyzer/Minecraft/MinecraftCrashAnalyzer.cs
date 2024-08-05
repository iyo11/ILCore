namespace ILCore.Analyzer.Minecraft;

public class MinecraftCrashAnalyzer
{
    public IEnumerable<string> Analyze(IEnumerable<MinecraftLog> minecraftLogs)
    {
        return minecraftLogs.Select(minecraftLog => minecraftLog.Message.Split(":")).Select(logs => logs[0] switch
        {
            "net.minecraftforge.fml.common.MissingModsException" => "Missing Mods",
            _ => "Unknown"
        });
    }
}