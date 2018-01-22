using System;
using Tauron.Application.Common.BaseLayer.Data;

namespace Tauron.Application.MgiProjectManager.Data.Entitys
{
    public class SetupEntity : GenericBaseEntity<int>
    {
        public int Value { get; set; }

        public SetupType SetupType { get; set; }

        public DateTime StartTime { get; set; }
    }
}