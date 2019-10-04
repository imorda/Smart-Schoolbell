using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNET_mvc_core
{
    public static class RandomUtils
    {
        private static Random _random;
        private static Random Random
        {
            get
            {
                if (_random == null)
                    _random = new Random();
                return _random;
            }
        }

        public static int Range(int from, int to)
        {
            return Random.Next(from, to);
        }
    }
}
