using System;
using System.Threading.Tasks;
using Anotar.Serilog;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Tauron.Application.Deployment.Server.Engine.Provider;
using Tauron.Application.Deployment.Server.Services.Validatoren;

namespace Tauron.Application.Deployment.Server.Services
{
    public sealed class RepositoryServiceImpl : RepositoryService.RepositoryServiceBase
    {
        private readonly IRepositoryManager _repositoryManager;

        public RepositoryServiceImpl(IRepositoryManager repositoryManager) 
            => _repositoryManager = repositoryManager;

        public override async Task GetProviders(GetRepositoryProviders request, IServerStreamWriter<RepositoryProvider> responseStream, ServerCallContext context)
        {
            try
            {
                foreach (var provider in _repositoryManager.Providers)
                {
                    await responseStream.WriteAsync(new RepositoryProvider
                                                    {
                                                        Description = provider.Description,
                                                        Id = provider.Id,
                                                        Name = provider.Name
                                                    });
                }
            }
            catch (Exception e)
            {
                LogTo.Error(e, "Error On Get Providers");
                throw;
            }
        }

        public override async Task GetRepositorys(RegistratedReporitorys request, IServerStreamWriter<RegistratedReporitory> responseStream, ServerCallContext context)
        {
            foreach (var reporitory in await _repositoryManager.GetAllRepositorys())
            {
                await responseStream.WriteAsync(new RegistratedReporitory
                                                {
                                                    Comment = reporitory.Comment,
                                                    Name = reporitory.Name,
                                                    Provider = new RepositoryProvider
                                                               {
                                                                   Description = reporitory.Provider.Description,
                                                                   Id = reporitory.Provider.Id,
                                                                   Name = reporitory.Provider.Name
                                                               }
                                                });
            }
        }

        [Authorize(AuthenticationSchemes = "Simple")]
        public override async Task<GenericResult> RegisterRepository(RegisterRepositoryData request, ServerCallContext context)
        {
            await RepositoryDataValidator.ForAsync(request);

            var (msg, ok) = await _repositoryManager.Register(request.Name, request.Provider, request.Source, request.Comment);

            return new GenericResult{ ErrorMessage = msg, Ok = ok};
        }

        [Authorize(AuthenticationSchemes = "Simple")]
        public override async Task<GenericResult> Delete(DeleteRepository request, ServerCallContext context)
        {
            await DeleteRepositoryValidator.ForAsync(request);
            var (msg, ok) = await _repositoryManager.Delete(request.Name);

            return new GenericResult
                   {
                       ErrorMessage = msg,
                       Ok = ok
                   };
        }
    }
}