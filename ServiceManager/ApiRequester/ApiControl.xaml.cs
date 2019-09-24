using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Logging;

namespace ServiceManager.ApiRequester
{
    /// <summary>
    /// Interaktionslogik für ApiControl.xaml
    /// </summary>
    public partial class ApiControl
    {
        private readonly ILogger<ApiControl> _logger;
        private readonly IApiRequester _apiRequester;

        public event Func<string, Task> KeyRecived;

        public ApiControl(ILogger<ApiControl> logger, IApiRequester apiRequester)
        {
            _logger = logger;
            _apiRequester = apiRequester;
            InitializeComponent();
        }

        private async void Send_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string key = await _apiRequester.RegisterApiKey(ServiceName.Text);

                if (string.IsNullOrEmpty(key))
                {
                    Dispatcher?.Invoke(() => Error.Text = "Der Schlüssel Ist Leer");
                    return;
                }

                Error.Text = $"Schlüssel: {key}";

                var invoker = KeyRecived;
                if( invoker == null) return;

                await invoker(key);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error on Api Request");
                Dispatcher?.Invoke(() => Error.Text = exception.Message);
            }
        }
    }
}
