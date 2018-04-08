using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using NLog;

namespace Tauron.Application.ProjectManager.Generic.Clients
{
    public abstract class ClientHelperBase<TChannel> : ClientBase<TChannel>
        where TChannel : class
    {
        protected ClientHelperBase(Binding binding, EndpointAddress adress)
            : base(binding, adress)
        {
            Logger = LogManager.GetCurrentClassLogger();
        }

        protected ClientHelperBase(InstanceContext context, Binding binding, EndpointAddress adress)
            : base(context, binding, adress)
        {
            Logger = LogManager.GetCurrentClassLogger();
        }

        protected Logger Logger { get; }

        protected TReturn Secure<TReturn>(Func<TReturn> action)
        {
            try
            {
                return action();
            }
            catch (Exception e)
            {
                if (CriticalExceptions.IsCriticalException(e))
                    throw;

                Logger.Log(LogLevel.Error, e);

                throw new ClientException($"{e.GetType()} - {e.Message}", e);
            }
        }

        protected void Secure(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                if (CriticalExceptions.IsCriticalException(e))
                    throw;

                Logger.Log(LogLevel.Error, e);

                throw new ClientException($"{e.GetType()} - {e.Message}", e);
            }
        }
    }
}