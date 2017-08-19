using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Random_Cut_Forest.src.Additional
{
    static class DoubleTools
    {
        public static decimal Sum(IEnumerable<double> list)
        {
            /*
             *     Method return correct sum of elemens in double collection
             *     
             *     used:
             *          - for choice random proportional dimention algorithm
             */
            decimal d = 0;
            foreach (var e in list)
            {
                d += (decimal)e;
            }
            return d;
        }

        public static decimal Div(double a, decimal b)
        {
            /*
             *    Return correct value of division double and decimal
             */
            return (decimal)a / b;
        }

    }
}
