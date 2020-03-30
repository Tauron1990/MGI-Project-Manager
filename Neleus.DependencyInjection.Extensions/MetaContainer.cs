using System;

namespace Neleus.DependencyInjection.Extensions
{
    internal sealed class MetaContainer<TMeta>
    {
        public Type Implementation { get; }

        public TMeta Meta { get; set; }

        public MetaContainer(Type tmplementation)
        {
            Implementation = tmplementation;
        }
    }
}