using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nololiyt.Captcha.Extensions
{
    internal static class RandomValueGenerator
    {
        private static readonly Random rootRandom = new Random();
        private static readonly object locker = new object();

        public static Random GetRandom()
        {
            int seed;
            lock (locker)
            {
                seed = rootRandom.Next(int.MinValue, int.MaxValue);
            }
            return new Random(seed);
        }
    }
}
