using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Grpc.Core;
using Tauron.Application.Deployment.Server.Services.Validatoren;

namespace Tauron.Application.Deployment.Server.Services
{
    public class ErrorPushSubscriptionImpl : ErrorPushSubscription.ErrorPushSubscriptionBase
    {
        private readonly SubscribeManager _manager;

        public ErrorPushSubscriptionImpl(SubscribeManager manager) 
            => _manager = manager;

        public override async Task SubscribeSyncError(Registration request, IServerStreamWriter<SyncError> responseStream, ServerCallContext context)
        {
            await RegistrationValidation.ForAsync(request);
            
            var (ok, block) = _manager.Add(request.Name);
            if (!ok)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Registrien Existiert schon"));

            while (await block.OutputAvailableAsync()) 
                await responseStream.WriteAsync(await block.ReceiveAsync());
        }

        public override async Task<Registration> UnSubscribeSyncError(Registration request, ServerCallContext context)
        {
            await RegistrationValidation.ForAsync(request);

            _manager.Remove(request.Name);

            return request;
        }
    }
}