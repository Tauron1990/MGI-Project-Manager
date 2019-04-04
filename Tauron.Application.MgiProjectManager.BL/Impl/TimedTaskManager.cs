using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tauron.Application.MgiProjectManager.BL.Contracts;
using Tauron.Application.MgiProjectManager.Server.Data.Entitys;
using Tauron.Application.MgiProjectManager.Server.Data.Impl;
using Tauron.Application.MgiProjectManager.Server.Data.Repository;

namespace Tauron.Application.MgiProjectManager.BL.Impl
{
    public class TimedTaskManager : ITimedTaskManager
    {
        private readonly ITimedTaskRepository _taskRepository;
        private readonly ILogger<TimedTaskRepository> _logger;
        private readonly Timer _timer;
        private readonly ITimeTask[] _timeTasks;

        private Task _intTask;
        private ConcurrentDictionary<string, TimedTaskEntity> _list;
        
        public TimedTaskManager(IEnumerable<ITimeTask> tasks, ITimedTaskRepository taskRepository, ILogger<TimedTaskRepository> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
            _timeTasks = tasks.ToArray();
            _timer = new Timer(TriggerTasks, null, -1, -1);
            _intTask = taskRepository.GetTaskAsync().ContinueWith(t => _list = new ConcurrentDictionary<string, TimedTaskEntity>(t.Result.ToDictionary(ks => ks.Name)));
        }
        
        private async void TriggerTasks(object state)
        {
            try
            {
                if (_intTask != null)
                {
                    _intTask.Wait();
                    _intTask.Dispose();
                    _intTask = null;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Get TimeTask Entitys. Set List to null");
                if (_list == null)
                    _list = null;
            }

            foreach (var timeTask in _timeTasks.Where(NeedRun))
            {
                try
                {
                    await timeTask.TriggerAsync();
                    _list[timeTask.Name] = await _taskRepository.UpdateTime(timeTask.Name);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error Time Task");
                }
            }
        }

        private bool NeedRun(ITimeTask task)
        {
            if (_list.TryGetValue(task.Name, out var entity))
                return entity.LastRun + task.Interval < DateTime.Now;
            return true;
        }

        public void Start() 
            => _timer.Change(TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10));
    }
}