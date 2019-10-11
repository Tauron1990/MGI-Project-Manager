using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceManager.Installation.Tasks;

namespace ServiceManager.Installation.Core
{
    public sealed class InstallerProcedure : INotifyPropertyChanged
    {
        private readonly ILogger<InstallerProcedure> _logger;
        private InstallerTask _currentTask;

        public InstallerProcedure(ILogger<InstallerProcedure> logger) => _logger = logger;

        public void InitInstall(IServiceProvider serviceProvider)
        {
            Tasks.Add(TaskCreator<NameSelectionTask>(serviceProvider));
            Tasks.Add(TaskCreator<CopyTask>(serviceProvider));
            Tasks.Add(TaskCreator<ApiRequestingTask>(serviceProvider));
            Tasks.Add(TaskCreator<StartTask>(serviceProvider));
        }

        public void InitUpdate(IServiceProvider serviceProvider)
        {
            Tasks.Add(TaskCreator<StopTask>(serviceProvider));
            Tasks.Add(TaskCreator<SelectUpdateTask>(serviceProvider));
            Tasks.Add(TaskCreator<StartTask>(serviceProvider));
        }

        public ObservableCollection<InstallerTask> Tasks { get; } = new ObservableCollection<InstallerTask>();

        public InstallerTask CurrentTask
        {
            get => _currentTask;
            set
            {
                if (Equals(value, _currentTask)) return;
                _currentTask = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task<string> Install(InstallerContext context)
        {
            var rollback = new List<InstallerTask>();
            string error = null;

            foreach (var installerTask in Tasks)
            {
                try
                {
                    await installerTask.Prepare(context);
                    rollback.Add(installerTask);

                    installerTask.Running = true;

                    await Task.Delay(1_000);

                    CurrentTask = installerTask;
                    error = await installerTask.RunInstall(context);

                    if (string.IsNullOrEmpty(error))
                    {
                        if (CurrentTask != null)
                            CurrentTask.Running = false;
                        CurrentTask = null;
                        continue;
                    }

                    await RollBack(rollback, context);
                    return error;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error on Install: {context.ServiceName}");
                    await RollBack(rollback, context);
                    error = e.Message;
                    break;
                }
            }

            return error;
        }

        private async Task RollBack(IEnumerable<InstallerTask> tasks, InstallerContext installerContext)
        {
            try
            {
                foreach (var installerTask in tasks) await installerTask.Rollback(installerContext);

                _logger.LogInformation($"Rollback Compled: {installerContext.ServiceName}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error on Rollback: {installerContext.ServiceName}");
            }
        }

        private static InstallerTask TaskCreator<TType>(IServiceProvider context)
            where TType : InstallerTask =>
            ActivatorUtilities.CreateInstance<TType>(context);

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}