using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomCutForest.src.Additional
{
    public static class RandomTools
    {
        //static int seed = 423561;
        //static Random rdm = new Random(seed);
        static Random rdm = new Random();
        static object key = new object();



        public static decimal RandomDecimal(decimal min, decimal max)
        {
            /*
             *      Return random decimal in some range.
             *      Thread safe
             */
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

        public static int RandomInt(int min, int max)
        {
            /*
             *     Return random integer.
             *     Thread safe
             */
            int r;
            lock (key)
            {
                r = rdm.Next(min, max);
            }
            return r;
        }
    }
}
