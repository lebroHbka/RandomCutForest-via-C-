using System;

namespace RandomCutForest.Additional
{
    /// <summary>
    /// Class that help working with random, make it thread safe
    /// </summary>
    public static class RandomTools
    {
        static Random rdm = new Random();
        static object key = new object();

        /// <summary>
        /// Return random decimal in some range.
        /// </summary>
        /// <param name="min">Min value(inclusive)</param>
        /// <param name="max">Max value(exclusive)</param>
        /// <returns></returns>
        public static decimal RandomDecimal(decimal min, decimal max)
        {
            decimal randValue;

            if (min == max)
                return min;

            if ((max - min) <= 1)
            {
                lock (key)
                {
                    randValue = (decimal)rdm.NextDouble();
                }
                while (randValue >= max)
                {
                    randValue -= max;
                }
                return randValue + min;
            }
            lock (key)
            {
                randValue = (decimal)((rdm.Next((int)min, (int)max)) + rdm.NextDouble());
            }
            return randValue;
        }

        /// <summary>
        /// Return random integer.
        /// </summary>
        /// <param name="min">Min value(inclusive)</param>
        /// <param name="max">Max value(exclusive)</param>
        /// <returns></returns>
        public static int RandomInt(int min, int max)
        {
            int r;
            lock (key)
            {
                r = rdm.Next(min, max);
            }
            return r;
        }
    }
}
