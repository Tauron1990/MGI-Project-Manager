using System;
using System.Runtime.Serialization;
using Tauron.Application.MgiProjectManager.Properties;
using Tauron.Application.MgiProjectManager.ServiceLayer.Dto;

namespace Tauron.Application.MgiProjectManager.UI
{
    [Serializable]
    public class RunTimeCalculatorItem : ObservableObject, ISerializable
    {
        private DateTime _endTime;
        private DateTime _startTime;

        public RunTimeCalculatorItem(RunTimeCalculatorItemType itemType, DateTime startTime)
        {
            StartTime = startTime;
            ItemType  = itemType;

            switch (ItemType)
            {
                case RunTimeCalculatorItemType.Iteration:
                    EndTime = startTime + TimeSpan.FromMinutes(Settings.Default.IterationTime);
                    break;
                case RunTimeCalculatorItemType.Setup:
                    EndTime = startTime + TimeSpan.FromMinutes(Settings.Default.SetupTime);
                    break;
                case RunTimeCalculatorItemType.Running:
                    EndTime = startTime + TimeSpan.FromHours(1);
                    break;
            }
        }

        protected RunTimeCalculatorItem(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            StartTime = (DateTime) info.GetValue(nameof(StartTime), typeof(DateTime));
            EndTime   = (DateTime) info.GetValue(nameof(EndTime), typeof(DateTime));
            ItemType  = (RunTimeCalculatorItemType) info.GetValue(nameof(ItemType), typeof(RunTimeCalculatorItemType));
        }

        public DateTime StartTime
        {
            get => _startTime;
            set => SetProperty(ref _startTime, value);
        }

        public DateTime EndTime
        {
            get => _endTime;
            set => SetProperty(ref _endTime, value);
        }

        public RunTimeCalculatorItemType ItemType { get; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(StartTime), StartTime);
            info.AddValue(nameof(EndTime), EndTime);
            info.AddValue(nameof(ItemType), ItemType);
        }

        public TimeSpan? CalculateDiffernce()
        {
            if (EndTime < StartTime) return null;

            return EndTime - StartTime;
        }

        public AddSetupInputItem CreateDto()
        {
            return new AddSetupInputItem(_startTime, _endTime, ItemType);
        }
    }
}