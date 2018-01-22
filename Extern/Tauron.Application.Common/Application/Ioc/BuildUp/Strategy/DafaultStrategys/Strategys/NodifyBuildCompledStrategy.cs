// The file NodifyBuildCompledStrategy.cs is part of Tauron.Application.Common.
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

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The nodify build compled strategy.</summary>
    public class NodifyBuildCompledStrategy : StrategyBase
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The on post build.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        public override void OnPostBuild(IBuildContext context)
        {
            var notify = context.Target as INotifyBuildCompled;

            if (notify == null) return;

            context.ErrorTracer.Phase = "Notify Build Compled for " + context.Metadata;
            notify.BuildCompled();
        }

        #endregion
    }
}