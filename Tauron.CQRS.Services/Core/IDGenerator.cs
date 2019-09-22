using System;
using Be.Vlaanderen.Basisregisters.Generators.Guid;
using JetBrains.Annotations;
using SequentialGuid;

namespace Tauron.CQRS.Services.Core
{
    [PublicAPI]
    public sealed class IdGenerator
    {
        public static IdGenerator Generator = new IdGenerator();

        private IdGenerator()
        {
            
        }

        public Guid NewGuid() 
            => Guid.NewGuid();

        public Guid NewGuid(Guid namespaceGuid, string name, int version = 5) 
            => Deterministic.Create(namespaceGuid, name, version);

        public Guid NewSequenqialGuid()
            => SequentialGuidGenerator.Instance.NewGuid();
    }
}