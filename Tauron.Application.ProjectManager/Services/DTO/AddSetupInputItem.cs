using System;
using JetBrains.Annotations;

namespace Tauron.Application.ProjectManager.Services.DTO
{
    [PublicAPI, Serializable]
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
}