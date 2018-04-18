namespace Tauron.Application.Implementation.Controls
{
    /// <summary>
    ///     Interaktionslogik für SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen
    {
        public SplashScreen()
        {
            InitializeComponent();
            #if(DEBUG)
            ShowInTaskbar = true;
            #else
			ShowInTaskbar = false;
#endif
        }
    }
}