using System.Linq;
using Tauron.Application.Ioc.Components;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The instantiation strategy.</summary>
    public class InstantiationStrategy : StrategyBase
    {
        /// <summary>The _service.</summary>
        private IProxyService _service;

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="components">
        ///     The components.
        /// </param>
        public override void Initialize(ComponentRegistry components)
        {
            _service = components.Get<IProxyService>();
        }

        /// <summary>
        ///     The on create instance.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        public override void OnCreateInstance(IBuildContext context)
        {
            var policy = context.Policys.Get<ConstructorPolicy>();
            if (policy == null) return;

            context.ErrorTracer.Phase = "Contruct Object for " + context.Metadata;

            context.Target = policy.Constructor.Invoke(context, policy.Generator); //(context, policy.Generator);
        }

        /// <summary>
        ///     The on perpare.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        public override void OnPerpare(IBuildContext context)
        {
            if (context.Target != null) return;

            context.ErrorTracer.Phase = "Reciving Construtor Informations for " + context.Metadata;

            context.Policys.Add(
                new ConstructorPolicy
                {
                    Constructor =
                        context.UseInternalInstantiation()
                            ? Helper.WriteDefaultCreation(context)
                            : context.Metadata.Export.ExternalInfo.Create,
                    Generator =
                        _service.Generate(context.Metadata, context.Metadata.Export.ImportMetadata.ToArray(),
                            out var interceptor)
                });

            if (interceptor == null) return;

            var pol = context.Policys.Get<ExternalImportInterceptorPolicy>();

            if (pol != null)
            {
                pol.Interceptors.Add(interceptor);
            }
            else
            {
                pol = new ExternalImportInterceptorPolicy();
                pol.Interceptors.Add(interceptor);

                context.Policys.Add(pol);
            }
        }

    }
}