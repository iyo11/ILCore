using System.Diagnostics;
using ILCore.Analyzer.Minecraft;

namespace ILCore.Launch
{
    public interface IProcessStart
    {
        Task Start();
    }

    public class MinecraftProcessStart(Process process) : IProcessStart
    {
        public async Task Start()
        {
            process.Start();
            process.BeginOutputReadLine();
            await process.WaitForExitAsync();
        }
    }
    
    
    
    public class MinecraftProcess
    {
        private readonly MinecraftLogAnalyzer _minecraftLogAnalyzer = new();
        private readonly MinecraftCrashAnalyzer _minecraftCrashAnalyzer = new();
        
        public MinecraftProcessStart PrepareProcess(string javaPath, string arguments)
        {
            List<MinecraftLog> minecraftCrashLogs = [];
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
            
            process.OutputDataReceived += async (_, args) =>
            {
                var log = await _minecraftLogAnalyzer.Analyze(args.Data);
                if (log.Type is MinecraftLogType.Fatal or MinecraftLogType.Error)
                {
                    minecraftCrashLogs.Add(log);
                }
            };
            
            process.Exited += (_, args) =>
            {
                var crashes = _minecraftCrashAnalyzer.Analyze(minecraftCrashLogs);
                
            };
            
            return  new MinecraftProcessStart(process);
        }
    }
}
