using System;
using System.Windows.Input;
using Tauron.Application.ProjectManager.Services.Data.Entitys;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.MgiProjectManager.UI
{
    public sealed class JobItem : ObservableObject
    {
        private bool _importent;
        private string _longName;
        private string _name;
        private JobStatus _status;
        private DateTime _targetDate;

        public JobItem()
        {
        }

        public JobItem(JobItemDto dto)
        {
            Importent = dto.Importent;
            Name = dto.Name;
            LongName = dto.LongName;
            TargetDate = dto.TargetDate;
            Status = dto.Status;
        }

        public DateTime TargetDate
        {
            get => _targetDate;
            set => SetProperty(ref _targetDate, value);
        }

        public string LongName
        {
            get => _longName;
            set => SetProperty(ref _longName, value);
        }

        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value.Trim('-'));
                CurrentDispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
            }
        }

        public JobStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public bool Importent
        {
            get => _importent;
            set => SetProperty(ref _importent, value);
        }

        public JobItemDto CreateDto()
        {
            return new JobItemDto
            {
                Importent = Importent,
                LongName = LongName,
                Name = Name,
                Status = Status,
                TargetDate = TargetDate
            };
        }
    }
}