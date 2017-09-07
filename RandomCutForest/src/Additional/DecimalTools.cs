using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomCutForest.src.Additional
{
    public static class DecimalTools
    {
        public static bool Compare(decimal[] a, decimal[] b)
        {
            /*
             *      Compare array with value, order is mean
             */
            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }

        public static bool Contains(List<decimal[]> list, decimal[] element)
        {
            /*
             *      Check contain list element or no.
             */
            foreach (var p in list)
            {
                if (Compare(p, element))
                    return true;
            }
            return false;
        }

    }
}
