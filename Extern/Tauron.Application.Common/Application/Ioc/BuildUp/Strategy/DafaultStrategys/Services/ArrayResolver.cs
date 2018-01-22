// The file ArrayResolver.cs is part of Tauron.Application.Common.
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
//  along with Tauron.Application.Common If not, see <http://www.gnu.org/licenses/>.

#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public sealed class ArrayResolver : IResolver
    {
        private readonly IResolver[] _resolvers;
        private readonly Type _target;

        public ArrayResolver(IEnumerable<IResolver> resolvers, Type target)
        {
            _resolvers = resolvers.ToArray();
            _target = target;
        }

        public object Create(ErrorTracer errorTracer)
        {
            errorTracer.Phase = "Injecting Array for " + _target;

            try
            {
                var arr = Array.CreateInstance(_target, _resolvers.Length);

                var index = 0;

                foreach (var resolver in _resolvers)
                {
                    var val = resolver.Create(errorTracer);

                    if (errorTracer.Exceptional) return arr;

                    arr.SetValue(val, index);
                    index++;
                }

                return arr;
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
                return null;
            }
        }
    }
}