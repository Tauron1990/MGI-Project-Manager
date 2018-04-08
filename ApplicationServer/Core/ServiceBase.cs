using System;
using System.ServiceModel;
using NLog;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.ApplicationServer.Core
{
    public abstract class ServiceBase
    {
        protected ServiceBase()
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

                throw new FaultException<GenericServiceFault>(new GenericServiceFault(e.GetType(), e.Message));
            }
        }
    }
}