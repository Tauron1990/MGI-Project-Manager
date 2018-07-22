using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.ProjectManager.Services.DTO
{
    [Serializable, PublicAPI]
    public class AddSetupInput
    {
        public AddSetupInput(IEnumerable<AddSetupInputItem> items)
        {
            Items = new List<AddSetupInputItem>(items);
        }

        public List<AddSetupInputItem> Items { get; }
    }
}