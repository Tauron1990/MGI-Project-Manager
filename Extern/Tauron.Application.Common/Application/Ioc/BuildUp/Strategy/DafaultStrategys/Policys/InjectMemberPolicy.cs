#region

using System.Collections.Generic;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The inject member policy.</summary>
    public class InjectMemberPolicy : IPolicy
    {
        [NotNull]
        public ImportMetadata Metadata { get; set; }

        /// <summary>Gets or sets the injector.</summary>
        /// <value>The injector.</value>
        [NotNull]
        public MemberInjector Injector { get; set; }

        [CanBeNull]
        public List<IImportInterceptor> Interceptors { get; set; }
    }
}