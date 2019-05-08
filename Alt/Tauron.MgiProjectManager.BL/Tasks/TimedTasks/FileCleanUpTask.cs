using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Tauron.MgiProjectManager.BL.Services;
using Tauron.MgiProjectManager.Data;
using Tauron.MgiProjectManager.Dispatcher.Actions;

namespace Tauron.MgiProjectManager.BL.Tasks.TimedTasks
{
    [UsedImplicitly]
    public class FileCleanUpTask : ITimeTask
    {
        public string Name => nameof(FileCleanUpTask);
        public TimeSpan Interval => TimeSpan.FromDays(7);
        public async Task TriggerAsync(IServiceProvider serviceProvider)
        {
            var repository = serviceProvider.GetRequiredService<IUnitOfWork>();
            var fileManager = serviceProvider.GetRequiredService<IFileManager>();
            
            var files = await repository.FileRepository.GetUnRequestedFiles();
            var date = DateTime.Now - TimeSpan.FromDays(6 * 30);

            foreach (var fileEntity in files)
            {
                if(fileEntity.Age > date)
                    continue;

                await fileManager.DeleteFile(fileEntity.Path);
                await repository.FileRepository.DeleteFile(fileEntity.Id);
            }

            await repository.SaveChanges();
        }
    }
}