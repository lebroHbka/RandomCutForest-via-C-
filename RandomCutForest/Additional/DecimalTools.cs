using System.Collections.Generic;

namespace RandomCutForest.Additional
{
    /// <summary>
    /// Class that help working with decimal, that needed in alghoritm
    /// </summary>
    public static class DecimalTools
    {
        /// <summary>
        /// Compare array with value, order is mean(!)
        /// </summary>
        /// <param name="a">First array</param>
        /// <param name="b">Second array</param>
        /// <returns></returns>
        public static bool Compare(decimal[] a, decimal[] b)
        {
            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }

    }
}
