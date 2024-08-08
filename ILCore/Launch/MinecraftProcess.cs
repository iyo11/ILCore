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
            
            process.OutputDataReceived += (s,args) =>
            {
                var log = _minecraftLogAnalyzer.Analyze(args.Data);
                if (log.Type is MinecraftLogType.Error or MinecraftLogType.Fatal)
                    minecraftCrashLogs.Add(log);
                if (!string.IsNullOrEmpty(log.Message))
                {
                    MinecraftLogOutPut?.Invoke(s,log);
                }
            };
            process.ErrorDataReceived += (s,args) =>
            {
                var log = _minecraftLogAnalyzer.Analyze(args.Data);
                if (log.Type is MinecraftLogType.Error or MinecraftLogType.Fatal)
                    minecraftCrashLogs.Add(log);
                MinecraftLogOutPut?.Invoke(s,log);
            };
            
            process.Exited += (s,_) =>
            {
                var crash = _minecraftCrashAnalyzer.Analyze(minecraftCrashLogs)
                    .Where(item => !string.IsNullOrEmpty(item))
                    .GroupBy(item => item)
                    .SelectMany(group => group.Take(1));

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
