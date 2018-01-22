using System;
using System.Collections.Generic;
using Tauron.Application.MgiProjectManager.UI;

namespace Tauron.Application.MgiProjectManager.ServiceLayer.Dto
{
    public class AddSetupInputItem
    {
        private readonly DateTime _endTime;

        public AddSetupInputItem(DateTime startTime, DateTime endTime, RunTimeCalculatorItemType itemType)
        {
            StartTime = startTime;
            _endTime  = endTime;
            ItemType  = itemType;
        }

        public DateTime StartTime { get; }

        public RunTimeCalculatorItemType ItemType { get; }

        public TimeSpan? CalculateDiffernce()
        {
            if (_endTime < StartTime) return null;

            return _endTime - StartTime;
        }
    }

    public class AddSetupInput
    {
        public AddSetupInput(IEnumerable<AddSetupInputItem> items)
        {
            Items = new List<AddSetupInputItem>(items);
        }

        public List<AddSetupInputItem> Items { get; }
    }
}