using System;
using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Fluent.Impl
{
    internal class ConstructorConfiguration<TReturn> : IConstructorConfiguration<TReturn>
    {
        private readonly ObjectBuilder _context;
        private readonly TReturn _ret;

        public ConstructorConfiguration([NotNull] ObjectBuilder builder, TReturn ret)
        {
            _context = builder;
            _ret = ret;
        }

        public TReturn Apply()
        {
            return _ret;
        }

        public IConstructorConfiguration<TReturn> WhithConstructor(ConstructorInfo info)
        {
            _context.SetConstructor(info, -1);
            return this;
        }

        public IConstructorConfiguration<TReturn> WithCusomConstructor(Func<object?, object> constructor)
        {
            _context.BuilderFunc = constructor;
            return this;
        }

        public IConstructorConfiguration<TReturn> WithObject(object parm)
        {
            _context.CustomObject = parm;
            return this;
        }
    }
}