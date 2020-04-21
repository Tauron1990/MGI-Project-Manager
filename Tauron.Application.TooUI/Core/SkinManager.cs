using System.Windows;
using Scrutor;
using Syncfusion.SfSkinManager;

namespace Tauron.Application.ToolUI.Core
{
    [ServiceDescriptor(typeof(ISkinManager))]
    public sealed class SkinManager : ISkinManager
    {
        public void Apply(DependencyObject obj) 
            => SfSkinManager.SetVisualStyle(obj, VisualStyles.Blend);
    }
}