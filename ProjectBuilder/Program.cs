using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using Tauron.Application.Pipes;
using Tauron.Application.Pipes.IO;

namespace ProjectBuilder
{
    [MessagePackObject]
    public class BuildInfo
    {
        public const string BuildFile = "Data.build";

        [Key(0)]
        public string Output { get; }

        [Key(1)]
        public string PipeHandle { get; }

        [Key(2)]
        public string ProjectFile { get; }

        [SerializationConstructor]
        public BuildInfo(string output, string pipeHandle, string projectFile)
        {
            Output = output;
            PipeHandle = pipeHandle;
            ProjectFile = projectFile;
        }
    }

    static class Program
    {
        private static PipeServer<string> _uploadapp = PipeServer<string>.Empty;

        static async Task<int> Main()
        {
            try
            {
                var path = Path.GetFullPath(BuildInfo.BuildFile, Assembly.GetEntryAssembly()?.Location);
                if (!File.Exists(path))
                    return -1;

                await using var file = File.OpenRead(path);
                var info = await  MessagePackSerializer.DeserializeAsync<BuildInfo>(file);

                if (!string.IsNullOrWhiteSpace(info.PipeHandle) && info.PipeHandle != "none")
                {
                    _uploadapp = new PipeServer<string>(Anonymos.Create(PipeDirection.Out, info.PipeHandle));
                    await _uploadapp.Connect();
                }

                var projectName = info.ProjectFile;
                var output = info.Output;

                var arguments = new StringBuilder()
                    .Append(" publish ")
                    .Append($"\"{projectName}\"")
                    .Append($" -o \"{output}\"")
                    .Append(" -c Release")
                    .Append(" -v n");

                using var process = new Process();

                process.ErrorDataReceived += ProcessOnErrorDataReceived;
                process.OutputDataReceived += ProcessOnOutputDataReceived;
                process.EnableRaisingEvents = true;


                process.StartInfo = new ProcessStartInfo(@"C:\Program Files\dotnet\dotnet.exe", arguments.ToString())
                {
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                if (!process.WaitForExit(30000))
                    process.Kill(true);

                return process.ExitCode;
            }
            catch
            {
                return -1;
            }
        }

        private static async void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if(_uploadapp.CanWrite)
                await _uploadapp.SendMessage(e.Data);
        }

        private static async void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if(_uploadapp.CanWrite)
                await _uploadapp.SendMessage("error");
        }
    }
}
