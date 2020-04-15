using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace JKang.IpcServiceFramework.IntegrationTests
{
    public class TestService : ITestService
    {
        public Complex AddComplex(Complex x, Complex y) => x + y;

        public float AddFloat(float x, float y) => x + y;

        public void DoNothing()
        {
        }

        public DateTime ParseDate(string value, DateTimeStyles styles) => DateTime.Parse(value, CultureInfo.InvariantCulture, styles);

        public Complex SumComplexArray(IEnumerable<Complex> input)
        {
            var result = new Complex();
            foreach (var value in input) result += value;
            return result;
        }

        public byte[] ReverseBytes(byte[] input) => input.Reverse().ToArray();

        public T GetDefaultValue<T>() => default;

        public async Task<long> WaitAsync(int milliseconds)
        {
            var sw = Stopwatch.StartNew();
            await Task.Delay(milliseconds);
            return sw.ElapsedMilliseconds;
        }
    }
}