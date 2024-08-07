using System.Diagnostics;
using ILCore.Analyzer.Minecraft;

namespace ILCore.Launch
{
    public interface IProcessStart
    {
        Task Start();
    }

    public class MinecraftProcess : IProcessStart
    {
        public event EventHandler<MinecraftLog> MinecraftLogOutPut;
        public event EventHandler<IEnumerable<string>> MinecraftLogCrash;
        public Process Process { get; set; }
        private readonly MinecraftLogAnalyzer _minecraftLogAnalyzer = new();
        private readonly MinecraftCrashAnalyzer _minecraftCrashAnalyzer = new();
        
        public MinecraftProcess(Process process)
        {
            Process = process;
            List<MinecraftLog> minecraftCrashLogs = [];
            
            process.OutputDataReceived += async (s,args) =>
            {
                var log = await _minecraftLogAnalyzer.Analyze(args.Data);
                if (log.Type is MinecraftLogType.Error or MinecraftLogType.Fatal)
                    minecraftCrashLogs.Add(log);
                MinecraftLogOutPut?.Invoke(s,log);
            };
            process.ErrorDataReceived += async (s,args) =>
            {
                var log = await _minecraftLogAnalyzer.Analyze(args.Data);
                if (log.Type is MinecraftLogType.Error or MinecraftLogType.Fatal)
                    minecraftCrashLogs.Add(log);
                MinecraftLogOutPut?.Invoke(s,log);
            };
            
            process.Exited += (s,args) =>
            {
                foreach (var minecraftCrashLog in minecraftCrashLogs)
                {
                    Console.WriteLine(minecraftCrashLog.ToString());
                }
                var crash = _minecraftCrashAnalyzer.Analyze(minecraftCrashLogs);
                MinecraftLogCrash?.Invoke(s,crash);
            };
        }
        
        public async Task Start()
        {
            Process.Start();
            Process.BeginOutputReadLine();
            Process.BeginErrorReadLine();
            await Process.WaitForExitAsync();
        }
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
                    RedirectStandardInput = true,
                }
            };
            process.EnableRaisingEvents = true;
            return  new MinecraftProcess(process);
        }
    }
}
