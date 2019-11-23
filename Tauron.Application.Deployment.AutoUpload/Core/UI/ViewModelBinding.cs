using System.Windows;
using JetBrains.Annotations;

namespace Tauron.Application.Deployment.AutoUpload.Core.UI
{
    [PublicAPI]
    public sealed class ViewModelBinding : BindingDecoratorBase
    {
        public ViewModelBinding([NotNull] string path)
        {
            Converter = new ViewModelConverter();
            Path = new PropertyPath(path);
        }

        public ViewModelBinding()
            : this(string.Empty)
        {}
    }
}