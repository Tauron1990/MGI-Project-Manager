using System.ServiceModel;
using Tauron.Application.ProjectManager.ApplicationServer.Core;
using Tauron.Application.ProjectManager.Services;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.ApplicationServer.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class JobPushMessageImpl : ServiceBase, IJobPushMessage
    {
        public JobPushMessageImpl() => AllowedRights = UserRights.Manager;

        public void Ping() => Secure(() => ConnectivityManager.SendPong(OperationContext.Current.SessionId));

        public void Register()
        {
            var context = OperationContext.Current;
            Secure(() =>
                   {
                       ConnectivityManager.Register(context.SessionId, context.Channel, context.GetCallbackChannel<IJobPushMessageCallback>());
                       ConnectivityManager.SendPong(context.SessionId);
                   });
        }

        public void UnRegister() => Secure(() => ConnectivityManager.UnResgister(OperationContext.Current.SessionId));
    }
}