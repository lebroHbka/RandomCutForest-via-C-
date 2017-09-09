using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomCutForest.Additional;

namespace RandomCutForest.Components
{
    /// <summary>
    /// This class encapsulate multidimensional data points to find anomaly.
    /// (!) Point might be all unique(at least in one dimension with some value)
    /// </summary>
    public class Data
    {
        public static byte DimensionsCount;

        #region Vars

        List<decimal[]> PointsList { get; set; }
        public int Length { get { return PointsList.Count; } }

        List<decimal> boxMin, boxMax, boxLen;

        decimal SplitValue { get; set; }
        byte SplitDimension { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize data with list of points(array of coordinats)
        /// </summary>
        /// <param name="data"></param>
        public Data(List<decimal[]> data)
        {
            if (DimensionsCount == 0)
                throw new Exception("Set dimension count, before using Data");
            PointsList = data;
            CreateBoundBox();
        }

        #endregion

        #region Add / Remove points
        /// <summary>
        /// Add new points in curent points list.
        /// Update bounding box.
        /// </summary>
        /// <param name="data">New data that might be add</param>
        public void AddPoints(Data data)
        {
            PointsList.AddRange(data.PointsList);
            Update();
        }

        /// <summary>
        /// Remove point from current points list.
        /// Update bounding box.
        /// </summary>
        /// <param name="old">Points that might be delete</param>
        public void RemovePoints(Data old)
        {
            foreach (var point in old.PointsList)
            {
                int i = 0;
                while (i < PointsList.Count)
                {
                    if (DecimalTools.Compare(point, PointsList[i]))
                    {
                        PointsList.RemoveAt(i);
                        break;
                    }
                    i++;
                }
            }
            Update();
        }

        #endregion

        #region Bounding Box

        /// <summary>
        /// Method calculate bounding box for current points collection
        /// </summary>
        void CreateBoundBox()
        {
            boxMin = Enumerable.Repeat(decimal.MaxValue, DimensionsCount).ToList();
            boxMax = Enumerable.Repeat(decimal.MinValue, DimensionsCount).ToList();
            boxLen = Enumerable.Repeat(0M, DimensionsCount).ToList();

            // Finding min and max points value in each dimention to get bounding box
            for (int i = 0; i < DimensionsCount; i++)
            {
                foreach (var p in PointsList)
                {
                    boxMin[i] = (p[i] < boxMin[i]) ? p[i] : boxMin[i];
                    boxMax[i] = (p[i] > boxMax[i]) ? p[i] : boxMax[i];
                }
            }

            // Calculate bounding box lengths(max - min) of each dimention
            for (int i = 0; i < DimensionsCount; i++)
                boxLen[i] = boxMax[i] - boxMin[i];
        }

        /// <summary>
        /// Create new bounding box.
        /// </summary>
        public void Update()
        {
            CreateBoundBox();
        }

        #endregion

        #region Split

        /// <summary>
        /// Method choice random dimension(proportional to len) and random value for split
        /// </summary>
        void CreateSplitData()
        {
            // Get sum of all dimension and choice random number
            // All dimensions going one after one
            decimal splitValue = RandomTools.RandomDecimal(0, boxLen.Sum());

            decimal sum = 0;
            short i = 0;

            // Figure out in that dimension is splitValue(after cicle it will be i-1)
            do
            {
                sum += boxLen[i];
                i++;
            } while (sum < splitValue);

            // i==1 mean that splitValue in 0 dimension, simple calculation
            if (i == 1)
            {
                splitValue = boxMin[0] + splitValue;
                SplitDimension = 0;
            }
            // i > 1, to get split value in current dimension
            // need cut off all previos dimensions(thay going one after one) from sum 
            // and add to start position(min boundiong box)
            else
            {
                sum -= boxLen[i - 1];
                splitValue = boxMin[i - 1] + (splitValue - sum);
                SplitDimension = (byte)(i - 1);
            }
            SplitValue = splitValue;
        }

        /// <summary>
        /// Make two new points group after spliting.
        /// </summary>
        /// <param name="leftPoints">Points group that locate lower than split line</param>
        /// <param name="rightPoints">Points group that locate higher than split line</param>
        public void Split(out Data leftPoints, out Data rightPoints)
        {
            /*
             *    Alghoritm:
             *    1) Choose a random dimension proportional to l(i) / ( l(1) + ... + l(n))
             *       where l(i) = max x∈S  − min x∈S.
             *       Choose Xi ∼ Uniform[minx∈S xi, maxx∈S xi]
             *      
             *      Doing by <CreateSplitData> method.
             *      
             *    2) Split points between two groups: leftPoints, rightPoint
             *    
             */

            var left = new List<decimal[]>();
            var right = new List<decimal[]>();

            // Get random dimension and value to split
            CreateSplitData();

            // sort elements in group
            foreach (var point in PointsList)
            {
                if (point[SplitDimension] < SplitValue)
                    left.Add(point);
                else
                    right.Add(point);
            }

            leftPoints = new Data(left);
            rightPoints = new Data(right);
        }

        #endregion

        /// <summary>
        /// Return point with position==index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Data GetPoint(int index)
        {
            if ((index >= Length) || index < 0)
                throw new ArgumentOutOfRangeException("GetPoint index out of range");
            return new Data(new List<decimal[]> { PointsList[index] });
        }

        /// <summary>
        /// Check can be other point placed in current bounding box or no.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsInside(Data other)
        {
            foreach (var p in other.PointsList)
            {
                for (int i = 0; i < DimensionsCount; i++)
                {
                    if ((p[i] > boxMax[i]) || (p[i] < boxMin[i]))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Merge two Data object in one.
        /// </summary>
        /// <param name="first">First data to merge</param>
        /// <param name="second">Second data to merge</param>
        /// <returns>Merged data object</returns>
        public static Data Merge(Data first, Data second)
        {
            var points = first.PointsList.GetRange(0, first.Length);
            points.AddRange(second.PointsList);
            return new Data(points);
        }

        /// <summary>
        /// Return position of data, wich side data from split line("left"/"right")
        /// </summary>
        /// <param name="data"></param>
        /// <returns>"left"/"right"</returns>
        public string Position(Data data)
        {
            var point = data.PointsList.First();
            if (point[SplitDimension] < SplitValue)
                return "left";
            else
                return "right";
        }


        /// <summary>
        /// Compare points value of to Data object,ordering not mean.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Data))
                return false;

            var other = obj as Data;

            if (other.Length != this.Length)
                return false;

            foreach (var pOther in other.PointsList)
            {
                bool found = false;
                foreach (var pThis in this.PointsList)
                {
                    if(DecimalTools.Compare(pOther, pThis))
                    {
                        found = true;
                        break;

                    }
                }
                if (!found)
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            foreach (var point in PointsList)
            {
                str.Append("[");
                foreach (var coordinat in point)
                {
                    str.Append(coordinat + ", ");
                }
                str.Append("]");
            }
            return str.ToString();
        }
    }
}
