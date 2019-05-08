using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tauron.MgiProjectManager.Data;
using Tauron.MgiProjectManager.Data.Models;
using Tauron.MgiProjectManager.Dispatcher.Actions;

namespace Tauron.MgiProjectManager.Dispatcher
{
    [Export(typeof(ITimedTaskManager), LiveCycle = LiveCycle.Singleton)]
    public class TimedTaskManager : ITimedTaskManager
    {
        private readonly ILogger<ITimedTaskManager> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly Timer _timer;
        private readonly ITimeTask[] _timeTasks;

        private ConcurrentDictionary<string, TimedTaskEntity> _list;

        public TimedTaskManager(IEnumerable<ITimeTask> tasks, ILogger<ITimedTaskManager> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _timeTasks = tasks.ToArray();
            _timer = new Timer(TriggerTasks, null, -1, -1);
        }

        private async void TriggerTasks(object state)
        {
            try
            {
                using var work = _serviceProvider.CreateScope();

                if (_list == null)
                    _list = new ConcurrentDictionary<string, TimedTaskEntity>((await work
                        .ServiceProvider
                        .GetRequiredService<IUnitOfWork>()
                        .TimedTaskRepository
                        .GetTasks())
                        .ToDictionary(te => te.Name));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Get TimeTask Entitys. Set List to null");
                if (_list == null)
                    _list = new ConcurrentDictionary<string, TimedTaskEntity>();
            }

            foreach (var timeTask in _timeTasks)
            {
                try
                {
                    var (needRun, needCreate) = NeedRun(timeTask);

                    if (needRun) await UpdateEntry(timeTask, async t => await t.TriggerAsync(_serviceProvider));
                    else if(needCreate) await UpdateEntry(timeTask, null);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error Time Task");
                }
            }
        }

        private async Task UpdateEntry(ITimeTask timeTask, Func<ITimeTask, Task> exec)
        {
            using var scope = _serviceProvider.CreateScope();
            var work = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            if(exec != null)
                await exec.Invoke(timeTask);

            _list[timeTask.Name] = await work.TimedTaskRepository.UpdateTime(timeTask.Name);
            await work.SaveChanges();
        }

        private (bool NeedRun, bool NeedCreate) NeedRun(ITimeTask task)
        {
            return _list.TryGetValue(task.Name, out var entity) 
                ? (entity.LastRun + task.Interval < DateTime.Now, false) 
                : (false, true);
        }

        public void Start()
            => _timer.Change(TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10));
    }
}