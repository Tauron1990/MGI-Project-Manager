using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace TestConsoleApp
{
    class Margin
    {
        public int Top { get; set; }

        public int Buttom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }

        public static implicit operator Margin(ValueTuple<int, int, int, int> margin)
            => new Margin {Top = margin.Item1, Right = margin.Item2, Buttom = margin.Item3, Left = margin.Item4};
    }

    class Program
    {
        static void Main(string[] args)
        {
            Margin test = (10, 11, 12, 13);
            string test2 = "Retter";
            test2 = test2.ToLower();

            StringBuilder b = new StringBuilder();

            for (int i = test2.Length - 1; i >= 0; i--)
            {
                b.Append(test2[i]);
            }

            Console.WriteLine(test2 == b.ToString());
            Console.ReadKey();
        }
    }
}
