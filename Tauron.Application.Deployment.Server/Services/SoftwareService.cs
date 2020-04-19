using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Tauron.Application.Deployment.Server.Engine.Provider;
using Tauron.Application.Deployment.Server.Services.Validatoren;
using Tauron.Application.SoftwareRepo;

namespace Tauron.Application.Deployment.Server.Services
{
    public class SoftwareServiceImpl : SoftwareService.SoftwareServiceBase
    {
        private readonly IRepositoryManager _repositoryManager;

        public SoftwareServiceImpl(IRepositoryManager repositoryManager) 
            => _repositoryManager = repositoryManager;

        public override async Task GetApplications(ApplicationsRequest request, IServerStreamWriter<ApplicationEntry> responseStream, ServerCallContext context)
        {
            await ApplicationsRequestValidator.ForAsync(request);
            HashSet<string> filter = new HashSet<string>();

            async Task WriteEntrys(SoftwareRepository repo)
            {
                foreach (var app in repo.ApplicationList.ApplicationEntries.Where(ae => filter.Add(ae.Name)))
                {
                    await responseStream.WriteAsync(new ApplicationEntry
                                                    {
                                                        BranchName = app.BranchName,
                                                        Id = app.Id,
                                                        Last = app.Last.ToString(),
                                                        Name = app.Name,
                                                        RepositoryName = app.RepositoryName
                                                    });
                }
            }

            if (request.All)
            {
                foreach (var reporitory in await _repositoryManager.GetAllRepositorys())
                {
                    var (repo, _) = await _repositoryManager.Get(reporitory.Name);
                    if(repo == null) continue;

                    await WriteEntrys(repo);
                }
            }
            else
            {
                var (repo, msg) = await _repositoryManager.Get(request.Name);
                if(repo == null) 
                    throw new RpcException(new Status(StatusCode.InvalidArgument, msg));

                await WriteEntrys(repo);
            }
        }

        public override async Task<ApplicationResponse> GetApplication(ApplicationRequest request, ServerCallContext context)
        {
            await ApplicationRequestValidator.ForAsync(request);

            var entry = await FindApp(request.Name);

            if (entry == null)
            {
                return new ApplicationResponse
                       {
                           Error = new GenericResult
                                   {
                                       ErrorMessage = "Es wurde Keine Anwendung Gefunden",
                                       Ok = false
                                   }
                       };
            }

            return new ApplicationResponse
                   {
                       Entry = new ApplicationEntry
                               {
                                   BranchName = entry.BranchName,
                                   Id = entry.Id,
                                   Last = entry.Last.ToString(),
                                   Name = entry.Name,
                                   RepositoryName = entry.RepositoryName
                               }
                   };
        }

        public override async Task<CheckResponse> CheckUpdate(CheckRequest request, ServerCallContext context)
        {
            await CheckRequestValidator.ForAsync(request);

            try
            {
                var app = await FindApp(request.Name);
                if (app == null)
                {
                    return new CheckResponse
                           {
                               Error = true,
                               Message = "App Nicht Genfunden"
                           };
                }

                if(app.Last > Version.Parse(request.Version))
                {
                    var download = app.Downloads.Find(de => de.Version == app.Last);

                    return new CheckResponse
                           {
                               Error = false,
                               Update = true,
                               Entry = new DownloadEntry
                                       {
                                           Url = download.Url,
                                           Version = download.Version.ToString()
                                       }
                           };
                }

                return new CheckResponse
                       {
                           Error = false,
                           Update = false
                       };
            }
            catch (Exception e)
            {
                return new CheckResponse
                       {
                           Error = true,
                           Message = e.Message
                       };
            }
        }

        private async Task<SoftwareRepo.Data.ApplicationEntry?> FindApp(string name)
        {
            var repos = await _repositoryManager.GetAllRepositorys();

            foreach (var repo in repos)
            {
                var (softwareRepository, _) = await _repositoryManager.Get(repo.Name);

                var entry = softwareRepository?.ApplicationList.ApplicationEntries.FirstOrDefault(a => a.Name == name);
                if (entry != null) return entry;
            }

            return null;
        }
    }
}