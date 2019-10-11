using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceManager.ApiRequester;
using ServiceManager.Installation.Core;

namespace ServiceManager.Installation.Tasks
{
    public sealed class ApiRequestingTask : InstallerTask
    {
        private readonly Lazy<IApiRequester> _apiRequester;
        
        public override string Title => "Api Schlüssel";

        public ApiRequestingTask(Lazy<IApiRequester> apiRequester) 
            => _apiRequester = apiRequester;

        public override Task Prepare(InstallerContext context)
        {
            Content = "Api Schlüssel wird Abgerufen...";
            return base.Prepare(context);
        }

        public override async Task<string> RunInstall(InstallerContext context)
        {
            var key = await _apiRequester.Value.RegisterApiKey(context.ServiceName);

            if (string.IsNullOrWhiteSpace(key)) return "Es konnte kein Api Key Abgerufen werden!";

            var path = Path.Combine(context.InstalledPath, InstallerContext.ServiceSettingsFileName);

            if (!File.Exists(path)) return $"{InstallerContext.ServiceSettingsFileName} datei esisteirt nicht";

            var settings = JToken.Parse(await File.ReadAllTextAsync(path));

            settings["ApiKey"] = key;
            settings["Dispatcher"] = App.ClientCofiguration.BaseUrl;

            await using (var file = new FileStream(path, FileMode.Create))
            {
                await using var steamWriter = new StreamWriter(file);
                await steamWriter.WriteAsync(settings.ToString(Formatting.Indented));
            }

            return null;
        }
    }
}