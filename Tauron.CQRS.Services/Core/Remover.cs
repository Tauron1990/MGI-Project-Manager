using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Tauron.CQRS.Services.Core
{
    public static class Remover
    {
        private static readonly ConcurrentDictionary<Type, Func<object, object>> _cache = new ConcurrentDictionary<Type, Func<object, object>>();

        public static object Clear(this object obj) 
            => _cache.GetOrAdd(obj.GetType(), CreateClearer)(obj);

        private static Func<object, object> CreateClearer(Type arg)
        {
            var prop = arg.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
               .Where(p => p.IsDefined(typeof(RemoveAttribute))).ToArray();

            if (prop.Length == 0) return o => o;

            var parameter = Expression.Parameter(typeof(object));
            var castArg = Expression.Parameter(arg);
            var returnTarget = Expression.Label(typeof(object));

            var param = new[] {castArg};
            var expressions = new List<Expression>
                              {
                                  Expression.Assign(castArg, Expression.TypeAs(parameter, arg))
                              };

            expressions.AddRange(prop.Select(property => Expression.Assign(Expression.Property(castArg, property), Expression.Default(property.PropertyType))));

            expressions.Add(Expression.Return(returnTarget, Expression.TypeAs(castArg, typeof(object))));
            expressions.Add(Expression.Label(returnTarget, parameter));
            var block = Expression.Block(param, expressions);

            return Expression.Lambda<Func<object, object>>(block, true, parameter).Compile();
        }
    }
}