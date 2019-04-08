using System.Threading.Tasks;

namespace Tauron.Application.MgiProjectManager.BL.Contracts.Hubs
{
    public interface IFilesHub
    {
        Task SendMultifileProcessingCompled(string guid, bool error);
    }
}