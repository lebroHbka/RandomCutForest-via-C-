using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Random_Cut_Forest.src.Additional
{
    static class RandomTools
    {
        static Random rdm = new Random();
        static object key = new object();



        public static T Choice<T>(IEnumerable<T> list, IEnumerable<decimal> weights)
        {
            /*  
             *  Method choice 1 random element from list collection, chance proportional to probability in
             *  weights collection.
             *  
             *  Method works correct if weights total sum equal 1.0
             *  
             */
            if (list.Count() != weights.Count())
            {
                throw new InvalidOperationException("Different length of elements and weigth");
            }

            int index = 0;
            decimal rdmValue = (decimal)(RandomDouble());

            foreach (var w in weights)
            {
                if (rdmValue < w)
                {
                    return list.ElementAt(index);
                }
                rdmValue -= w;
                index++;
            }
            throw new InvalidOperationException("Total weight lower than 1.0");
        }

        public static double RandomDouble()
        {
            /*
             *   Method return random double in [0.0, 1.0)
             *   Thread safe
             */
            double d;
            lock (key)
            {
                d = rdm.NextDouble();
            }
            return d;
        }

        public static double RandomDouble(double min, double max)
        {
            /*
             *   Return random double in some range
             *   Thread safe
             */
            double randValue;

            if(((decimal)max - (decimal)min) <= 1)
            {
                lock (key)
                {
                    randValue = rdm.NextDouble();
                }
                randValue = (double)( (decimal)randValue + (decimal)min );
                return randValue;
            }

            lock (key)
            {
                randValue = (rdm.Next((int)min, (int)max)) + rdm.NextDouble();
            }
            return randValue;
        }

    }
}
