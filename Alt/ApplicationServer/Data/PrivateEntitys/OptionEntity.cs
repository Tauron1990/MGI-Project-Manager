using Tauron.Application.Common.BaseLayer.Data;

namespace Tauron.Application.ProjectManager.ApplicationServer.Data.PrivateEntitys
{
    public sealed class OptionEntity : GenericBaseEntity<string>
    {
        public string Content { get; set; }
    }
}