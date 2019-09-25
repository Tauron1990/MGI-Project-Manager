using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using ServiceManager.Installation.Tasks;

namespace ServiceManager.Installation.Core
{
    public class InstallerProcedure : INotifyPropertyChanged
    {
        private readonly ILogger<InstallerProcedure> _logger;
        private InstallerTask _currentTask;

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

        public InstallerProcedure(ILogger<InstallerProcedure> logger)
        {
            _logger = logger;

            Tasks.Add(new NameSelectionTask());
        }

        public async Task<string> Install(InstallerContext context)
        {
            List<InstallerTask> rollback = new List<InstallerTask>();
            string error = null;

            foreach (var installerTask in Tasks)
            {
                try
                {
                    await installerTask.Prepare(context);
                    rollback.Add(installerTask);

                    installerTask.Running = true;
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
                foreach (var installerTask in tasks) await installerTask.Rollback();

                _logger.LogInformation($"Rollback Compled: {installerContext.ServiceName}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error on Rollback: {installerContext.ServiceName}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}