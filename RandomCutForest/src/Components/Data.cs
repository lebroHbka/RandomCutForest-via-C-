using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RandomCutForest.src.Additional;

namespace RandomCutForest.src.Components
{
    public class Data
    {
        #region Vars
        public static byte DimensionsCount;

        public List<decimal[]> PointsList { get; set; }
        public int Length { get { return PointsList.Count; } }

        public List<decimal> BoxMin { get; private set; }
        public List<decimal> BoxMax { get; private set; }
        public List<decimal> BoxLen { get; private set; }

        public decimal SplitValue { get; set; }
        public short SplitDimension { get; set; }

        #endregion


        public Data(List<decimal[]> data)
        {
            PointsList = data;
            CreateBoundBox();
        }


        #region Add / Remove points
        public void AddPoints(Data data)
        {
            /*
             *      Add new points in curent points list.
             *      Not add point if this point already exists.
             *      Update bounding box.
             */

            foreach (var otherPoint in data.PointsList)
            {
                bool alreadyExist = false;
                foreach (var thisPoint in PointsList)
                {
                    if (DecimalTools.Compare(otherPoint, thisPoint))
                    {
                        alreadyExist = true;
                        break;
                    }
                }
                if (!alreadyExist)
                    PointsList.Add(otherPoint);
            }
            Update();
        }

        public void RemovePoints(Data old)
        {
            /*
             *      Remove point from current points list.
             *      Update bounding box.
             */
            foreach (var point in old.PointsList)
            {
                int i = 0;
                while (i < PointsList.Count)
                {
                    if (DecimalTools.Compare(point, PointsList[i]))
                    {
                        PointsList.RemoveAt(i);
                        continue;
                    }
                    i++;
                }
            }
            Update();
        }

        #endregion

        #region Bounding Box

        void CreateBoundBox()
        {
            /*
             *    Method calculate bounding box for current points collection
             */

            BoxMin = new List<decimal>();
            BoxMax = new List<decimal>();
            BoxLen = new List<decimal>();

            for (int i = 0; i < DimensionsCount; i++)
            {
                BoxMin.Add(decimal.MaxValue);
                BoxMax.Add(decimal.MinValue);
                BoxLen.Add(0);
            }

            // Finding min and max points value in each dimention to get bounding box
            for (int i = 0; i < DimensionsCount; i++)
            {
                foreach (var p in PointsList)
                {
                    BoxMin[i] = (p[i] < BoxMin[i]) ? p[i] : BoxMin[i];
                    BoxMax[i] = (p[i] > BoxMax[i]) ? p[i] : BoxMax[i];
                }
            }

            // Calculate bounding box lengths(max - min) of each dimention
            for (int i = 0; i < DimensionsCount; i++)
                BoxLen[i] = BoxMax[i] - BoxMin[i];
        }

        public void Update()
        {
            /*
             *      Create new bounding box.
             */
            CreateBoundBox();
        }

        #endregion

        #region Split

        void CreateSplitData()
        {
            /*
             *      Choice random dimension(proportional len) and random value 
             *      for split.
             *
             */

            // Get sum of all dimension and choice random number
            // All dimensions going one after one
            decimal splitValue = RandomTools.RandomDecimal(0, BoxLen.Sum());

            decimal sum = 0;
            short i = 0;

            // Figure out in that dimension is splitValue(after cicle it will be i-1)
            do
            {
                sum += BoxLen[i];
                i++;
            } while (sum < splitValue);

            // i==1 mean that splitValue in 0 dimension, simple calculation
            if (i == 1)
            {
                splitValue = BoxMin[0] + splitValue;
                SplitDimension = 0;
            }
            // i > 1, to get split value in current dimension
            // need cut off all previos dimensions(thay going one after one) from sum 
            // and add to start position(min boundiong box)
            else
            {
                sum -= BoxLen[i - 1];
                splitValue = BoxMin[i - 1] + (splitValue - sum);
                SplitDimension = (short)(i - 1);
            }
            SplitValue = splitValue;
        }

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

            List<decimal[]> left;
            List<decimal[]> right;
            do
            {
                left = new List<decimal[]>();
                right = new List<decimal[]>();
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
            }
            while ((left.Count == 0) || (right.Count == 0));

            leftPoints = new Data(left);
            rightPoints = new Data(right);
        }

        #endregion



        public bool IsInside(Data other)
        {
            /*
             *      Check can be other point placed in current bounding box or no.
             */
            foreach (var p in other.PointsList)
            {
                for (int i = 0; i < DimensionsCount; i++)
                {
                    if ((p[i] > BoxMax[i]) || (p[i] < BoxMin[i]))
                        return false;
                }
            }
            return true;
        }

        public static Data Merge(Data first, Data second)
        {
            /*
             *      Merge two Data object in one.
             *      Points will be merged.
             */
            var points = first.PointsList.GetRange(0, first.Length);
            foreach (var p in second.PointsList)
                if (!DecimalTools.Contains(points, p))
                    points.Add(p);
            return new Data(points);
        }

        public string Position(Data data)
        {
            var point = data.PointsList.First();
            if (point[SplitDimension] < SplitValue)
                return "left";
            else
                return "right";
        }



        public override bool Equals(object obj)
        {
            /*
             *      Compare points value of to Data object,
             *      ordering not mean.
             */
            if (!(obj is Data))
                return false;

            var other = obj as Data;

            if (other.Length != this.Length)
                return false;

            foreach (var otherPoint in other.PointsList)
            {
                var isConsist = false;
                foreach (var thisPoint in PointsList)
                {
                    if (DecimalTools.Compare(thisPoint, otherPoint))
                    {
                        isConsist = true;
                        break;
                    }
                }
                if (!isConsist)
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
            str.Append("\t\tbox: ");
            for (int i = 0; i < DimensionsCount; i++)
            {
                str.Append($"[{BoxMin[i]}, {BoxMax[i]}] ");
            }
            str.Append($"dim: {SplitDimension} sp: {SplitValue:0.###}");
            return str.ToString();
        }
    }
}
