using System.Collections.Generic;

namespace Tauron.Application.MgiProjectManager.BL.Contracts
{
    public interface IFileManager
    {
        (bool Ok, string Error) CanAdd(string name);

        (bool Ok, string Operation) AddFiles(IEnumerable<string> name);
    }
}