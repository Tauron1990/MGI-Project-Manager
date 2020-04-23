using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Catel.MVVM;
using Catel.Services;
using Scrutor;
using Tauron.Application.Wpf;
using Tauron.Application.Wpf.SerilogViewer;
using Path = Catel.IO.Path;

namespace Tauron.Application.ToolUI.ViewModels
{
    [ServiceDescriptor(typeof(LogEntryViewModel))]
    public sealed class LogEntryViewModel : ViewModelBase
    {
        private readonly IMessageService _messageService;

        public LogEntryViewModel(IMessageService messageService) 
            => _messageService = messageService;

        public ObservableCollection<LogEventViewModel> Events { get; set; } = new ObservableCollection<LogEventViewModel>();
        
        [CommandTarget]
        public async Task SaveLog()
        {
            try
            {
                var openFileDialog = new Ookii.Dialogs.Wpf.VistaSaveFileDialog
                                     {
                                        AddExtension = true,
                                        DefaultExt = ".log",
                                        Filter = "Txt|.txt|Json|.json|Log|.log",
                                        CheckFileExists = true,
                                        CheckPathExists = true,
                                        RestoreDirectory = true
                                     };

                if(openFileDialog.ShowDialog() == false) return;

                var filePath = openFileDialog.FileName;
                var dic = Path.GetDirectoryName(filePath);
                if (Directory.Exists(dic))
                    Directory.CreateDirectory(dic);

                await using var fileStream = new FileStream(filePath, FileMode.Create);
                await using var writer = new StreamWriter(fileStream);

                foreach (var model in Events) 
                    model.Info.EventInfo.RenderMessage(writer);
            }
            catch (Exception e)
            {
                await _messageService.ShowErrorAsync(e);
            }
        }
    }
}