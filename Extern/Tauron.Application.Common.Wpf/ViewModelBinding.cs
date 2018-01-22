using System.Windows;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public sealed class ViewModelBinding : BindingDecoratorBase
    {
        public ViewModelBinding([NotNull] string path)
        {
            Converter = new ViewModelConverter(false);
            Path = new PropertyPath(path);
        }

        public ViewModelBinding()
            : this(string.Empty)
        {
        }
    }
}