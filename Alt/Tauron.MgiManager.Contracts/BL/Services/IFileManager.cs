using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Tauron.MgiProjectManager.Model.Api;

namespace Tauron.MgiProjectManager.BL.Services
{
    public interface IFileManager
    {
        Task<(bool Ok, string Error)> CanAdd(string name);

        Task<(bool Ok, string Operation)> AddFiles(List<(string Name, Stream Stream)> files, string userName, Action<string> postError);

        Task DeleteFile(string path);

        Task<IEnumerable<UnAssociateFile>> GetUnAssociateFile(string opId);

        Task PostAssociateFile(AssociateFile file);

        Task<(bool Ok, string Error)> StartMultiFile(string id);
    }
}