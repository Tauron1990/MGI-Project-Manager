using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Tauron.Application.MgiProjectManager.BL.Contracts;
using Tauron.Application.MgiProjectManager.Server.Data.Repository;

namespace Tauron.Application.MgiProjectManager.BL.Impl.TimeTasks
{
    [UsedImplicitly]
    public class FileCleanUpTask : ITimeTask
    {
        private readonly IFileRepository _repository;
        private readonly IFileManager _fileManager;

        public FileCleanUpTask(IFileRepository repository, IFileManager fileManager)
        {
            _repository = repository;
            _fileManager = fileManager;
        }

        public string Name => nameof(FileCleanUpTask);
        public TimeSpan Interval => TimeSpan.FromDays(7);
        public async Task TriggerAsync()
        {
            var files = await _repository.GetUnRequetedFiles();
            var date = DateTime.Now - TimeSpan.FromDays(6 * 30);

            foreach (var fileEntity in files)
            {
                if(fileEntity.Age > date)
                    continue;

                await _fileManager.DeleteFile(fileEntity.Path);
                await _repository.DeleteFile(fileEntity.Id);
            }
        }
    }
}