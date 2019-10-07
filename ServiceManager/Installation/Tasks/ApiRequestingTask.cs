using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceManager.ApiRequester;
using ServiceManager.Installation.Core;

namespace ServiceManager.Installation.Tasks
{
    public class ApiRequestingTask : InstallerTask
    {
        private readonly Lazy<IApiRequester> _apiRequester;

        public override object Content => "Api Schlüssel wird Abgerufen...";

        public override string Title => "Api Schlüssel";

        public ApiRequestingTask(Lazy<IApiRequester> apiRequester) 
            => _apiRequester = apiRequester;

        public override async Task<string> RunInstall(InstallerContext context)
        {
            var key = await _apiRequester.Value.RegisterApiKey(context.ServiceName);

            if (string.IsNullOrWhiteSpace(key)) return "Es konnte kein Api Key Abgerufen werden!";

            var path = Path.Combine(context.InstalledPath, InstallerContext.SettingsFileName);

            if (!File.Exists(path)) return $"{InstallerContext.SettingsFileName} datei esisteirt nicht";

            var settings = JToken.Parse(await File.ReadAllTextAsync(path));

            settings["ApiKey"] = key;
            settings["Dispatcher"] = App.ClientCofiguration.BaseUrl;

            await using (var file = File.OpenWrite(path))
            {
                await using var steamWriter = new StreamWriter(file);
                await steamWriter.WriteAsync(settings.ToString(Formatting.Indented));
            }

            return null;
        }
    }
}