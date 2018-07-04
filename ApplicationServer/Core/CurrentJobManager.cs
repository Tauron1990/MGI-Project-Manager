using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.Common.BaseLayer.Data;
using Tauron.Application.ProjectManager.ApplicationServer.Data.Repositorys;
using Tauron.Application.ProjectManager.Services.Data.Entitys;

namespace Tauron.Application.ProjectManager.ApplicationServer.Core
{
    public static class CurrentJobManager
    {
        public static JobEntity CurrentJop { get; private set; }

        public static bool SetCurrentJob(string name)
        {
            using (var db = RepositoryFactory.Factory.Enter())
            {
                var ent = db.GetRepository<IJobRepository>().QueryAsNoTracking().Include(e => e.JobRuns).SingleOrDefault(e => e.Id == name);

                if (ent == null)
                {
                    CurrentJop = null;
                    return false;
                }

                CurrentJop = ent;
            }

            ConnectivityManager.Inform(c => c.CurrentJobChanged(name));
            return true;
        }

        public static bool SetStatus(JobStatus status)
        {
            if (CurrentJop == null)
                return false;

            using (var db = RepositoryFactory.Factory.Enter())
            {
                var ent = db.GetRepository<IJobRepository>().Find(CurrentJop.Id);
                if (ent == null) return false;

                ent.Status = status;
                CurrentJop.Status = status;
                db.SaveChanges();
            }

            return true;
        }
    }
}