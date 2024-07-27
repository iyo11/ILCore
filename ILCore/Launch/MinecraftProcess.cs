using System.Diagnostics;

namespace ILCore.Launch
{
    public class MinecraftProcess
    {
        public Process Launch(string javaPath, string arguments)
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
            process.Start();

            process.OutputDataReceived += (_, args) =>
            {
                Console.WriteLine(args.Data);
            };
            process.ErrorDataReceived += (_, args) =>
            {
                Console.WriteLine(args.Data);
            };

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            process.WaitForExit();
            process.Close();


            return process;
        }
    }
}
