using System;
using JetBrains.Annotations;
using NLog;
using Tauron.Application.Common.BaseLayer.BusinessLayer;

namespace Tauron.Application.MgiProjectManager.ServiceLayer.Impl
{
    public static class ExecutionHelper
    {
        private static bool LogIfNeed(IRuleBase rule, string name, string message = null)
        {
            if (!rule.Error) return true;

            var logger = LogManager.GetLogger(name);

            foreach (var ruleError in rule.Errors)
            {
                if (ruleError is Exception ex)
                    logger.Error(ex, message ?? ex.Message);
                else
                    logger.Error(ruleError);
            }

            return false;
        }

        public static TOutput ExecuteRule<TOutput>([NotNull] IOBussinesRule<TOutput> rule, TOutput defaultValue, string name, string message = null)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));

            var result = rule.Action();
            return LogIfNeed(rule, name, message) ? result : defaultValue;
        }

        public static TOutput ExecuteRule<TInput, TOutput>([NotNull] IIOBusinessRule<TInput, TOutput> rule, TInput input, TOutput defaultValue, string name, string message = null)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));

            var result = rule.Action(input);
            return LogIfNeed(rule, name, message) ? result : defaultValue;
        }

        public static void ExecuteRule<TInput>([NotNull] IIBusinessRule<TInput> rule, TInput input, string name, string message = null)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));

            rule.Action(input);
            LogIfNeed(rule, name, message);
        }

        public static void ExecuteRule([NotNull] IBusinessRule rule, string name, string message = null)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));

            rule.Action();
            LogIfNeed(rule, name, message);
        }
    }
}