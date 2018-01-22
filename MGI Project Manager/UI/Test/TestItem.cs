using System;

namespace Tauron.Application.MgiProjectManager.UI.Test
{
    public class TestItem
    {
        public DateTime TargetDate { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }

        public bool Importent { get; set; }

        public string TestDate
        {
            get => TargetDate.ToString();
            set => TargetDate = DateTime.Parse(value);
        }
    }
}