using System.Windows.Data;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Wpf.Converter;

namespace Tauron.Application.Deployment.AutoUpload.Core.Converter
{
    
    public class VersionRepositoryConverter : ValueConverterFactoryBase
    {
        protected override IValueConverter Create() 
            => CreateStringConverter<VersionRepository>(vr => vr?.Name ?? string.Empty);
    }
}