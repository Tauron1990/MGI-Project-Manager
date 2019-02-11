using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.Common.BaseLayer.Data;
using Tauron.Application.ProjectManager.ApplicationServer.Data.Repositorys;
using Tauron.Application.ProjectManager.Services.Data.Entitys;

namespace Tauron.Application.ProjectManager.ApplicationServer.Core
{
    public static class CurrentJobManager
    {
        public static JobEntity CurrentJob { get; private set; }

        public static bool SetCurrentJob(string name)
        {
            if (string.IsNullOrEmpty(name))
                CurrentJob = null;
            else
            {
                using (var db = RepositoryFactory.Factory.Enter())
                {
                    var ent = db.GetRepository<IJobRepository>().QueryAsNoTracking().Include(e => e.JobRuns).SingleOrDefault(e => e.Id == name);

                    if (ent == null)
                    {
                        CurrentJob = null;
                        return false;
                    }

                    CurrentJob = ent;
                }
            }

            ConnectivityManager.Inform(c => c.CurrentJobChanged(name));
            return true;
        }
        
        public static bool SetStatus(string name, JobStatus status)
        {
            using (var db = RepositoryFactory.Factory.Enter())
            {
                var ent = db.GetRepository<IJobRepository>().Find(name);
                if (ent == null) return false;

                ent.Status = status;
                CurrentJob.Status = status;
                db.SaveChanges();
            }

            return true;
        }
    }
}