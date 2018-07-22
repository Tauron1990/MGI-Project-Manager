using System;
using System.Threading.Tasks;
using NLog;
using Syncfusion.Windows.Controls.Notification;
using Tauron.Application.Models;

namespace Tauron.Application.MgiProjectManager.UI.Model
{
    [ExportModel(AppConststands.TaskManager)]
    public sealed class TaskManagerModel : ModelBase
    {
        private object _lock = new object();
        private bool _isLocked;

        public static readonly ObservableProperty AnimationTypeProperty = RegisterProperty("AnimationType", typeof(TaskManagerModel), typeof(AnimationTypes));

        public AnimationTypes AnimationType
        {
            get => GetValue<AnimationTypes>(AnimationTypeProperty);
            set => SetValue(AnimationTypeProperty, value);
        }

        public static readonly ObservableProperty IsBusyProperty = RegisterProperty(nameof(IsBusy), typeof(TaskManagerModel), typeof(bool), 
            new ObservablePropertyMetadata((p, m, v) => m.OnPropertyChanged(nameof(IsNotBusy))));

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

        public bool IsNotBusy => !IsBusy;

        public string Message
        {
            get => GetValue<string>(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public Task RunTask(Action action, string title, AnimationTypes animationType = AnimationTypes.Flower)
        {
            return Task.Run(() =>
            {
                AnimationType = animationType;
                Message = title;
                IsBusy = true;
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

        public void Lock(string title, AnimationTypes animationType = AnimationTypes.Flower)
        {
            lock (_lock)
            {
                _isLocked = true;
                Message = title;
                AnimationType = animationType;
                IsBusy = true;
            }
        }

        public void UnLock()
        {
            if (!IsBusy || !_isLocked) return;

            lock (_lock)
            {
                if(!_isLocked) return;

                _isLocked = false;
                IsBusy = false;
            }
        }
    }
}