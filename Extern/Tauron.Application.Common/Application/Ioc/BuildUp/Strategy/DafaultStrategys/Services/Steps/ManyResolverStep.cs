using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Tauron.Application.SimpleWorkflow;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Steps
{
    public abstract class ManyResolverStep : InjectorStep
    {
        private Type _currentType;
        private ExportEnumeratorHelper _enumeratorHelper;
        private Type _listType;
        private List<IResolver> _resolvers;

        public override StepId OnExecute(InjectorContext context)
        {
            _listType = context.ReflectionContext.CurrentType;
            _currentType = GetCurrentType(context.ReflectionContext);
            context.ReflectionContext.CurrentType = _currentType;

            var findAllExports = context.ReflectionContext.FindAllExports();
            if (findAllExports == null) return StepId.Invalid;

            _resolvers = new List<IResolver>();
            _enumeratorHelper = new ExportEnumeratorHelper(findAllExports.GetEnumerator(), context.ReflectionContext);
            return StepId.Loop;
        }

        [NotNull]
        protected abstract Type GetCurrentType([NotNull] ReflectionContext context);

        public override StepId NextElement(InjectorContext context)
        {
            if (context.Resolver != null)
                _resolvers.Add(context.Resolver);
            if (_enumeratorHelper.MoveNext())
                context.ReflectionContext.CurrentType = _currentType;
            return _enumeratorHelper.NextId;
        }

        [NotNull]
        protected abstract IResolver CreateResolver([NotNull] IEnumerable<IResolver> resolvers, [NotNull] Type listType);

        public override void OnExecuteFinish(InjectorContext context)
        {
            context.Resolver = CreateResolver(_resolvers, _listType);

            _resolvers = null;
            _listType = null;
            _enumeratorHelper = null;
        }
    }
}