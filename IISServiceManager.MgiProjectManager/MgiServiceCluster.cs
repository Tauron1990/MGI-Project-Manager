using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using IISServiceManager.Contratcs;
using IISServiceManager.MgiProjectManager.Resources;
using Microsoft.Web.Administration;

namespace IISServiceManager.MgiProjectManager
{
    public class MgiServiceCluster : IWebServiceCluster
    {
        private readonly XElement _configuration;
        private string _targetPath;

        public string Name { get; } = Strings.MgiServiceClusterName;

        public string Id { get; } = nameof(MgiServiceCluster);

        public IClusterConfig Config { get; }

        public MgiServiceCluster()
        {
            _configuration = XElement.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ServiceConfig.xml"));

            Config = new XmlClusterConfig(_configuration);
        }

        public Task<IEnumerable<IWebService>> GetServices()
            => Task.FromResult((
                                   from xElement in _configuration.Element("Services")?.Elements("Service") ?? Enumerable.Empty<XElement>()
                                   select new XmlWebService(xElement)
                               ).Cast<IWebService>());

        public Task<object> CheckPrerequisites()
        {
            _targetPath = Environment.GetEnvironmentVariable("Path")?.Split(';').FirstOrDefault(p => p.Contains("dotnet"));

            if (string.IsNullOrEmpty(_targetPath)) return Task.FromResult((object)Strings.MgiServiceCluster_NoDotnet);

            string sdkPath = Path.Combine(_targetPath, "sdk");

            if(!Directory.Exists(sdkPath) || Directory.EnumerateDirectories(sdkPath).FirstOrDefault(s => s.StartsWith("3")) == null)
                return Task.FromResult((object)Strings.MgiServiceCluster_NoDotnet);

            return Task.FromResult((object) null);
        }

        private string BuildCommandLine(string repoLocation, IWebService webService, string binayrPath)
        {
            string projectPath = Path.Combine(repoLocation, ((XmlWebService)webService).ProjectName);

            return $"publish {projectPath} --configuration {_configuration.Attribute("Configuration")?.Value ?? "Release"} --output {binayrPath}";
        }

        public Task<bool> Build(string repoLocation, string targetPath, IWebService service, ILog log)
        {
            log.WriteLine("Start dotnet Build");

            Process process = new Process
            {
                StartInfo =
                {
                    FileName = Path.Combine(_targetPath, "dotnet.exe"),
                    Arguments = BuildCommandLine(repoLocation, service, repoLocation),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            //* Set your output and error (asynchronous) handlers
            process.OutputDataReceived += (sender, args) => log.WriteLine(args.Data);
            process.ErrorDataReceived += (sender, args) => log.WriteLine(args.Data);
            //* Start process and handlers
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                log.WriteLine("Build Successful");
                return Task.FromResult(true);
            }

            log.AutoClose = false;
            log.WriteLine($"Build Failed: ExitCode-{process.ExitCode}");
            return Task.FromResult(false);
        }

        public Task PrepareServer(ServerManager manager, ILog log) 
            => Task.CompletedTask;

        public Task<ApplicationPool> GetAppPool(ApplicationPoolCollection applicationPools)
        {
            string name = _configuration.Attribute("AppPool")?.Value ?? "MGI";

            var pool = applicationPools[name];
            if (pool != null)
                return Task.FromResult(pool);

            pool = applicationPools.Add(name);
            pool.AutoStart = true;
            pool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
            pool.ManagedRuntimeVersion = "";

            return Task.FromResult(pool);
        }
    }
}