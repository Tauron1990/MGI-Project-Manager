using System.Linq;
using Tauron.Application.Ioc;
using Tauron.Application.MgiProjectManager.LocalCache;
using Tauron.Application.Models;
using Tauron.Application.ProjectManager.Generic;
using Tauron.Application.ProjectManager.Services;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.MgiProjectManager.UI.Model
{
    [ExportModel(AppConststands.JobManagerModel)]
    public class JobManagerModel : ModelBase
    {
        public static readonly ObservableProperty SelectedItemProperty = RegisterProperty("SelectedItem", typeof(JobManagerModel), typeof(JobItem));

        public static readonly ObservableProperty CurrentJobProperty = RegisterProperty("CurrentJob", typeof(JobManagerModel), typeof(JobItem));

        public JobItem SelectedItem
        {
            get => GetValue<JobItem>(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public JobItem CurrentJob
        {
            get => GetValue<JobItem>(CurrentJobProperty);
            set => SetValue(CurrentJobProperty, value);
        }

        [Inject]
        public ServiceManager ServiceManager { get; set; }

        public UISyncObservableCollection<JobItem> JobItems { get; private set; }

        public IJobManager JobManager { get; private set; }

        public override void BuildCompled()
        {
            JobManager = ServiceManager.CreateClint<IJobManager>();
            JobItems = new UISyncObservableCollection<JobItem>();
        }

        public bool Connect()
        {
            var ok = CacheManager.TakeCare<IJobManager, JobItemDto[]>(JobManager.GetActiveJobs, out var items);
            if (!ok) return false;

            JobItems.Clear();
            JobItems.AddRange(items.Select(d => new JobItem(d)));

            return true;
        }

        public bool SetCurrentJob()
        {
            string name = SelectedItem?.Name;
            
            if (!CacheManager.TakeCare<IJobManager, bool, string>(JobManager.SeCurrentJob, name, out var ok))
                return false;

            if (ok)
                CurrentJob = SelectedItem;

            return true;
        }

        public bool MarkImportent()
        {
            if (SelectedItem == null) return true;

            if (!CacheManager.TakeCare<IJobManager, JobItemDto>(JobManager.MarkImportent, SelectedItem.CreateDto()))
                return false;

            SelectedItem.Importent = true;
            return true;
        }
    }
}