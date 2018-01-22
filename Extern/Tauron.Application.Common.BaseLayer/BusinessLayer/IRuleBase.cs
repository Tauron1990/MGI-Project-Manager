using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Common.BaseLayer.BusinessLayer
{
    [PublicAPI]
    public interface IRuleBase
    {
        bool Error { get; }

        IEnumerable<object> Errors { get; }

        string InitializeMethod { get; }

        object GenericAction(object input);
    }
}