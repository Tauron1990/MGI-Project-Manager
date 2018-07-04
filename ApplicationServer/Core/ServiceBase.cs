using System;
using System.ServiceModel;
using JetBrains.Annotations;
using NLog;
using Tauron.Application.Common.BaseLayer.BusinessLayer;
using Tauron.Application.ProjectManager.Resources;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.ApplicationServer.Core
{
    [PublicAPI]
    public abstract class ServiceBase
    {
        private static RuleFactory _ruleFactory;

        protected static RuleFactory RuleFactory => _ruleFactory ?? (_ruleFactory = CommonApplication.Current.Container.Resolve<RuleFactory>());

        public UserRights? AllowedRights { get; set; }

        protected ServiceBase()
        {
            Logger = LogManager.GetCurrentClassLogger();
        }

        protected Logger Logger { get; }

        protected TReturn Secure<TReturn>(Func<TReturn> action, UserRights? rights = null, string methodName = null)
        {
            try
            {
                Authorization(rights, methodName);

                return action();
            }
            catch (Exception e)
            {
                if (CriticalExceptions.IsCriticalException(e) || e is FaultException)
                    throw;

                Logger.Log(LogLevel.Error, $"{action.Method.Name} - {e.Message}");

                throw new FaultException<GenericServiceFault>(new GenericServiceFault(e.GetType(), e.Message), e.Message);
            }
        }

        protected void Secure(Action action, UserRights? rights = null, string methodName = null)
        {
            try
            {
                Authorization(rights, methodName);

                action();
            }
            catch (Exception e)
            {
                if (CriticalExceptions.IsCriticalException(e) || e is FaultException)
                    throw;

                Logger.Log(LogLevel.Error, $"{action.Method.Name} - {e.Message}");

                throw new FaultException<GenericServiceFault>(new GenericServiceFault(e.GetType(), e.Message), e.Message);
            }
        }

        private void Authorization(UserRights? userRights, string methodName)
        {
            UserRights realRights;
            if (userRights != null)
                realRights = userRights.Value;
            else if (AllowedRights != null)
                realRights = AllowedRights.Value;
            else
                return;
            
            var name = OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name;
            if (!UserManager.HasRights(name, realRights, out var rights))
                throw new FaultException(new FaultReason($"{ServiceMessages.Authorization_Error}-{rights}->{realRights}"));
        }

        private bool LogIfNeed(IRuleBase rule, string name, string message = null)
        {
            if (!rule.Error) return true;

            foreach (var ruleError in rule.Errors)
            {
                if (ruleError is Exception ex)
                    Logger.Error(ex, message ?? ex.Message);
                else
                    Logger.Error(ruleError);
            }

            return false;
        }

        public TOutput ExecuteRule<TOutput>([NotNull] IOBussinesRule<TOutput> rule, TOutput defaultValue, string name, string message = null, UserRights? rights = null)
        {
            return Secure(() =>
                          {
                              var result = rule.Action();
                              return LogIfNeed(rule, name, message) ? result : defaultValue;
                          }, rights);
        }

        public TOutput ExecuteRule<TInput, TOutput>([NotNull] IIOBusinessRule<TInput, TOutput> rule, TInput input, TOutput defaultValue, string name, string message = null, UserRights? rights = null)
        {
            return Secure(() =>
                          {
                              var result = rule.Action(input);
                              return LogIfNeed(rule, name, message) ? result : defaultValue;
                          }, rights, name);
        }

        public void ExecuteRule<TInput>([NotNull] IIBusinessRule<TInput> rule, TInput input, string name, string message = null, UserRights? rights = null)
        {
            Secure(() =>
                   {
                       rule.Action(input);
                       LogIfNeed(rule, name, message);
                   }, rights);
        }

        public void ExecuteRule([NotNull] IBusinessRule rule, string name, string message = null, UserRights? rights = null)
        {
            Secure(() =>
                   {
                       rule.Action();
                       LogIfNeed(rule, name, message);
                   }, rights);
        }
    }
}