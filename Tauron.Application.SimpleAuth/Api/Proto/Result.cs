using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Google.Rpc;
using Serilog.Parsing;

namespace Tauron.Application.SimpleAuth.Api.Proto
{
    public static class Result
    {
        private abstract class ConstructorBase
        {
            public abstract object Create(string message, Code? code);
            public abstract object Create(Action<object> configurator);
        }

        private class GenericConstructor<TType> : ConstructorBase
            where TType : new()
        {
            private readonly Action<TType, Status> _statusSetter;

            public GenericConstructor(Action<TType, Status> statusSetter)
            {
                _statusSetter = statusSetter;
            }

            public override object Create(string message, Code? code)
            {
                var target = new TType();
                _statusSetter(target, new Status { Code = (int)(code ?? Code.Internal)});
                return target;
            }

            public override object Create(Action<object> configurator)
            {
                var target = new TType();
                configurator(target);
                return target;
            }
        }

        private static ConcurrentDictionary<Type, ConstructorBase> _constructors = new ConcurrentDictionary<Type, ConstructorBase>();

        public static void Register<TType>(Action<TType, Status> setter) 
            where TType : new() 
            => _constructors[typeof(TType)] = new GenericConstructor<TType>(setter);

        public static TType Fail<TType>(Exception e) 
            => Fail<TType>(e.Message);

        public static TType Fail<TType>(string message, Code? code = null)
        {
            if (_constructors.TryGetValue(typeof(TType), out var constructor))
                return (TType) constructor.Create(message, code);

            throw new InvalidOperationException("No Result type Found");
        }

        public static TType Create<TType>(Action<TType> config)
        {
            if (_constructors.TryGetValue(typeof(TType), out var constructor))
            {
                return (TType)constructor.Create(o => config((TType)o));
            }

            throw new InvalidOperationException("No Result type Found");
        }

        public static Task<TType> FailAsync<TType>(Exception e)
            => FailAsync<TType>(e.Message);
        public static Task<TType> FailAsync<TType>(string message, Code? code = null)
        {
            if (_constructors.TryGetValue(typeof(TType), out var constructor))
                return Task.FromResult((TType)constructor.Create(message, code));

            throw new InvalidOperationException("No Result type Found");
        }

        public static Task<TType> CreateAsync<TType>(Action<TType> config)
        {
            if (_constructors.TryGetValue(typeof(TType), out var constructor))
            {
                return Task.FromResult((TType)constructor.Create(o => config((TType)o)));
            }

            throw new InvalidOperationException("No Result type Found");
        }
    }
}