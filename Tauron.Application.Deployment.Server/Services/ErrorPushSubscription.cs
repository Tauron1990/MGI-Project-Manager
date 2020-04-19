using System.Threading;
using System.Threading.Tasks;
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

            using var token = new CancellationTokenSource();

            if (!_manager.Add(responseStream, token, request.Name))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Registrien Existiert schon"));

            while (!token.IsCancellationRequested) 
                await Task.Delay(1000, token.Token);
        }

        public override async Task<Registration> UnSubscribeSyncError(Registration request, ServerCallContext context)
        {
            await RegistrationValidation.ForAsync(request);

            _manager.Remove(request.Name);

            return request;
        }
    }
}