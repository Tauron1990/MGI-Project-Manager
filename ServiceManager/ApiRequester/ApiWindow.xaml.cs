namespace ServiceManager.ApiRequester
{
    /// <summary>
    /// Interaktionslogik für ApiWindow.xaml
    /// </summary>
    public partial class ApiWindow
    {
        public string Key { get; set; }

        public ApiWindow(ApiControl apiControl, MainWindow mainWindow)
        {
            InitializeComponent();

            Owner = mainWindow;
            Content = apiControl;
            apiControl.KeyRecived += async s =>
            {
                Key = s;
                if (Dispatcher != null) 
                    await Dispatcher.InvokeAsync(() => DialogResult = true);
            };
        }
    }
}
