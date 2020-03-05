using MessagePack;

namespace Tauron.Application.Deployment.AutoUpload.Models.Build
{
    [MessagePackObject]
    public class BuildInfo
    {
        public const string BuildFile = "Data.build";

        [SerializationConstructor]
        public BuildInfo(string output, string pipeHandle, string projectFile)
        {
            Output = output;
            PipeHandle = pipeHandle;
            ProjectFile = projectFile;
        }

        [Key(0)] public string Output { get; }

        [Key(1)] public string PipeHandle { get; }

        [Key(2)] public string ProjectFile { get; }
    }
}