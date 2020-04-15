// The file FluentConstructorConfiguration.cs is part of Tauron.Application.Files.
// 
// CoreEngine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CoreEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//  
// You should have received a copy of the GNU General Public License
//  along with Tauron.Application.Files If not, see <http://www.gnu.org/licenses/>.

#region

using System;
using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core.Managment;

#endregion

namespace Tauron.Application.Files.Serialization.Core.Fluent.Impl
{
    internal class ConstructorConfiguration<TReturn> : IConstructorConfiguration<TReturn>
    {
        private readonly ObjectBuilder _context;
        private readonly TReturn       _ret;

        public ConstructorConfiguration([NotNull] ObjectBuilder builder, TReturn ret)
        {
            _context = builder;
            _ret     = ret;
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

        public IConstructorConfiguration<TReturn> WithCusomConstructor(Func<object, object> constructor)
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