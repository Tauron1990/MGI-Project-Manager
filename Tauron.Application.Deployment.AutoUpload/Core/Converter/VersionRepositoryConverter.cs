using System.Windows.Data;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Wpf.Converter;

namespace Tauron.Application.Deployment.AutoUpload.Core.Converter
{
    public class VersionRepositoryConverter : ValueConverterFactoryBase
    {
        protected override IValueConverter Create()
        {
            return CreateStringConverter<VersionRepository>(vr => $"Repo {vr?.Name ?? string.Empty}");
        }
    }
}