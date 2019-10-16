using System;
using System.Linq.Expressions;
using CQRSlite.Domain.Exception;

namespace Tauron.CQRS.Services.Core.Components
{
    internal static class AggregateFactory<T>
    {
        private static readonly Func<T> Constructor = CreateTypeConstructor();

        private static Func<T> CreateTypeConstructor()
        {
            try
            {
                return Expression.Lambda<Func<T>>(Expression.New(typeof(T)), Array.Empty<ParameterExpression>()).Compile();
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public static T CreateAggregate()
        {
            if (Constructor == null)
            {
                throw new MissingParameterLessConstructorException(typeof(T));
            }
            return Constructor();
        }
    }
}