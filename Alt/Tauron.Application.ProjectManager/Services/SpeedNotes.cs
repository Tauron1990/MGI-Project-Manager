using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MathNet.Numerics;

namespace Tauron.Application.ProjectManager.Services
{
    [PublicAPI]
    public class SpeedNotesHolder
    {
        [PublicAPI]
        public const string DefaultRow1FileName = "SpeedRow1.Notes";

        [PublicAPI]
        public const string DefaultRow2FileName = "SpeedRow2.Notes";

        private readonly SpeedNotes _row1;
        private readonly SpeedNotes _row2;

        public SpeedNotesHolder()
            : this(DefaultRow1FileName, DefaultRow2FileName)
        {
        }

        private SpeedNotesHolder(string row1, string row2)
        {
            _row1 = new SpeedNotes(row1);
            _row2 = new SpeedNotes(row2);
        }

        public double CalculateSpeed(int rowCount, int mikron)
        {
            if (mikron < 12) return 0;
            if (rowCount == 1 && mikron > 100) return 0;
            if (rowCount == 2 && mikron > 200) return 0;

            if (rowCount == 1) return _row1.CalculateSpeed(mikron);
            if (rowCount == 2) return _row2.CalculateSpeed(mikron);

            return 0;
        }
    }

    [PublicAPI]
    public class SpeedNotes
    {
        public class SpeedNode
        {
            public double Speed { get; set; }

            public int Mikron { get; set; }
        }

        private readonly string _filePath;

        public SpeedNotes(string filePath)
        {
            _filePath = filePath;
            Nodes     = new List<SpeedNode>();

            if (!File.Exists(filePath)) return;

            var ok = false;

            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        var node = new SpeedNode();

                        var line = reader.ReadLine();

                        var segments = line?.Split(new[] {'-'}, 2, StringSplitOptions.RemoveEmptyEntries);

                        if (segments?.Length != 2)
                            return;

                        if (!double.TryParse(segments[0], out var speed)) return;
                        if (!int.TryParse(segments[1], out var drops)) return;

                        node.Speed  = speed;
                        node.Mikron = drops;

                        Nodes.Add(node);
                    }

                    ok = true;
                }
            }
            finally
            {
                if (!ok)
                    Nodes.Clear();
            }
        }

        public List<SpeedNode> Nodes { get; }

        public double CalculateSpeed(int mikron)
        {
            if (Nodes.Count < 2) return 0;

            try
            {
                return Math.Round(Interpolate.Linear(Nodes.Select(n => (double) n.Mikron), Nodes.Select(n => n.Speed)).Interpolate(mikron), 3);
            }
            catch (ArgumentException)
            {
                return 0;
            }
        }

        public void Save()
        {
            using (var stream = new StreamWriter(new FileStream(_filePath, FileMode.Create)))
            {
                foreach (var node in Nodes)
                {
                    stream.WriteLine(node.Speed + "-" + node.Mikron);
                }
            }
        }
    }
}