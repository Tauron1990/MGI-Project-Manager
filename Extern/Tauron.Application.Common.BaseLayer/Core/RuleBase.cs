using System.Collections.Generic;
using JetBrains.Annotations;
using Tauron.Application.Common.BaseLayer.BusinessLayer;
using Tauron.Application.Common.BaseLayer.Data;

namespace Tauron.Application.Common.BaseLayer.Core
{
    [PublicAPI]
    public abstract class RuleBase : IRuleBase
    {
        [InjectRepositoryFactory]
        public RepositoryFactory RepositoryFactory { get; set; }

        public bool Error { get; private set; }
        public IEnumerable<object> Errors { get; private set; }

        public virtual string InitializeMethod { get; }
        public abstract object GenericAction(object input);

        protected internal void SetError(params object[] errors)
        {
            SetError((IEnumerable<object>) errors);
        }

        protected void SetError(IEnumerable<object> objects)
        {
            Error = objects != null;
            Errors = objects != null ? new ReadOnlyEnumerator<object>(objects) : null;
        }
    }
}