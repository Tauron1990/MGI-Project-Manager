using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Tauron.Application.MgiProjectManager.Server.Core
{
    public interface IRazorPartialToStringRenderer
    {
        Task<string> RenderPartialToStringAsync(PartialViewResult partialName);
    }
}
