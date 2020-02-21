using System.IO;
using System.Threading.Tasks;

namespace FileServer.Lib.Abstractions
{
    public interface IFileAccess
    {
        Task<bool> ExisFile(string path);

        Task<Stream> OpenWrite(string path, bool createNew);
        void DeleteFile(string path);
    }
}