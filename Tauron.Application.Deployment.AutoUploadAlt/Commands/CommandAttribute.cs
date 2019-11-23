using System;
using JetBrains.Annotations;

namespace Tauron.Application.Deployment.AutoUpload.Commands
{
    [AttributeUsage(AttributeTargets.Class), BaseTypeRequired(typeof(CommandBase)), MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class CommandAttribute : Attribute
    {
        public string Name { get; }

        public string Description { get; }

        public CommandAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}