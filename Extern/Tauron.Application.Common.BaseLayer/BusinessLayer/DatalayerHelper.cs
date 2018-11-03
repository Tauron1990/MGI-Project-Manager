using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.Common.BaseLayer.Data;

namespace Tauron.Application.Common.BaseLayer.BusinessLayer
{
    public static class DatalayerHelper
    {
        private static readonly Dictionary<string, InitInfo> Infos = new Dictionary<string, InitInfo>();

        public static void InitializeRule(IRuleBase rule, RepositoryFactory factory)
        {
            if (rule is RuleBase baseRule) baseRule.SetError(null);

            if (string.IsNullOrWhiteSpace(rule.InitializeMethod)) return;

            var info = GetOrCreate(rule.GetType(), rule.InitializeMethod);

            info.MethodInfo.Invoke(rule, info.Type.Select(t =>
            {
                if (t == typeof(RepositoryFactory)) return factory;
                return factory.GetRepository(t);
            }).ToArray());
        }

        private static InitInfo GetOrCreate(Type type, string name)
        {
            var key = type.FullName + "+" + name;

            if (Infos.TryGetValue(key, out var cachedInfo)) return cachedInfo;

            var initInfo = new InitInfo {MethodInfo = type.GetMethod(name)};
            if (initInfo.MethodInfo == null) throw new InvalidOperationException($"Initialization method in Rule:{type.FullName} not Found");

            initInfo.Type.AddRange(initInfo.MethodInfo.GetParameterTypes());
            Infos[key] = initInfo;

            return initInfo;
        }

        private class InitInfo
        {
            public MethodInfo MethodInfo { get; set; }

            public List<Type> Type { get; } = new List<Type>();
        }
    }
}