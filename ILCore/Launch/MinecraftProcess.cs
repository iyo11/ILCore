using System.Diagnostics;
using ILCore.Analyzer.Minecraft;

namespace ILCore.Launch;

public interface IProcessStart
{
    Task Start();
}

public class MinecraftProcess : IProcessStart
{
    private readonly MinecraftCrashAnalyzer _minecraftCrashAnalyzer = new();
    private readonly MinecraftLogAnalyzer _minecraftLogAnalyzer = new();


    public MinecraftProcess(Process process)
    {
        Process = process;
        var isLaunched = false;
        List<MinecraftLog> minecraftCrashLogs = [];

        process.OutputDataReceived += (s, args) =>
        {
            var log = _minecraftLogAnalyzer.Analyze(args.Data);
            if (string.IsNullOrEmpty(log.Message)) return;
            if (log.Type is MinecraftLogType.Error or MinecraftLogType.Fatal)
                minecraftCrashLogs.Add(log);
            MinecraftLogOutPut?.Invoke(log);
            if (isLaunched) return;
            if (!log.Message.Contains("Setting user:")) return;
            MinecraftLaunchSuccess?.Invoke();
            isLaunched = true;
        };
        process.ErrorDataReceived += (s, args) =>
        {
            var log = _minecraftLogAnalyzer.Analyze(args.Data);
            if (string.IsNullOrEmpty(log.Message)) return;
            if (log.Type is MinecraftLogType.Error or MinecraftLogType.Fatal)
                minecraftCrashLogs.Add(log);
            MinecraftLogOutPut?.Invoke(log);
        };

        process.Exited += (s, _) =>
        {
            var crash = _minecraftCrashAnalyzer.Analyze(minecraftCrashLogs)
                .Where(item => !string.IsNullOrEmpty(item))
                .GroupBy(item => item)
                .SelectMany(group => group.Take(1)).ToList();
            if (!isLaunched)
                MinecraftLaunchFailed?.Invoke();
            if (crash.Count != 0)
                MinecraftLogCrash?.Invoke(crash);
        };
    }

    public Process Process { get; set; }

    public async Task Start()
    {
        Process.Start();
        Process.BeginOutputReadLine();
        Process.BeginErrorReadLine();
        await Process.WaitForExitAsync();
    }

    public event Action<MinecraftLog> MinecraftLogOutPut;
    public event Action MinecraftLaunchSuccess;
    public event Action MinecraftLaunchFailed;
    public event Action<IEnumerable<string>> MinecraftLogCrash;
}

public class MinecraftProcessBuilder
{
    public MinecraftProcess BuildProcess(string javaPath, string arguments)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo(javaPath)
            {
                UseShellExecute = false,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true
            }
        };
        process.EnableRaisingEvents = true;
        return new MinecraftProcess(process);
    }
}