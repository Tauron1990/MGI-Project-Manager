using System.IO;
using System.Windows.Data;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Wpf.Converter;

namespace Tauron.Application.Deployment.AutoUpload.Core.Converter
{
    public sealed class RegistratedRepositoryConverter : ValueConverterFactoryBase
    {
        protected override IValueConverter Create()
        {
            return CreateStringConverter<RegistratedRepository>(
                r => $"Repo: {r?.RepositoryName} -- Branch: {r?.BranchName} -- Projekt: {Path.GetFileNameWithoutExtension(r?.ProjectName)}");
        }
    }
}