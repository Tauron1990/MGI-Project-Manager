using System.Threading.Tasks;

namespace Tauron.MgiProjectManager.BL.Hubs
{
    public interface IFilesHub
    {
        Task SendMultifileProcessingCompled(string guid, bool error, string msg);

        Task SendLinkingCompled(string guid, bool ok, string msg);
    }
}