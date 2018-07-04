using System;
using System.Threading.Tasks;
using NLog;
using Tauron.Application.Models;

namespace Tauron.Application.MgiProjectManager.UI.Model
{
    [ExportModel(AppConststands.TaskManager)]
    public sealed class TaskManagerModel : ModelBase
    {
        public static readonly ObservableProperty IsBusyProperty = RegisterProperty(nameof(IsBusy), typeof(TaskManagerModel), typeof(bool));

        public static readonly ObservableProperty MessageProperty = RegisterProperty(nameof(Message), typeof(TaskManagerModel), typeof(string));

        public TaskManagerModel()
        {
            LogCategory = AppConststands.MainPatternName;
        }

        public bool IsBusy
        {
            get => GetValue<bool>(IsBusyProperty);
            set => SetValue(IsBusyProperty, value);
        }

        public string Message
        {
            get => GetValue<string>(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public async Task RunTask(Action action, string title)
        {
            await Task.Run(() =>
                           {
                               Message = title;
                               IsBusy  = true;
                               Log.Write("Enter Task", LogLevel.Debug);
                               try
                               {
                                   action();
                               }
                               catch (Exception e)
                               {
                                   Log.Logger.Error(e);
                                   throw;
                               }
                               finally
                               {
                                   Message = string.Empty;
                                   Log.Write("Exit Task", LogLevel.Debug);
                                   IsBusy = false;
                               }
                           });
        }
    }
}