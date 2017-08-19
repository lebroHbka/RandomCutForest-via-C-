using Random_Cut_Forest.src.Additional;
using System;
using System.Collections.Generic;
using System.Text;

namespace Random_Cut_Forest.src
{
    class Data 
    {
        static int dimensionsCount = 2;

        // [x, y]
        public List<double[]> PointsCoordinates{ get; private set; }
        public int PointsCount { get { return PointsCoordinates.Count; } }


        public List<double> BoundingBoxMin { get; private set; }
        public List<double> BoundingBoxMax { get; private set; }
        public List<double> BoundingBoxLength { get; private set; }

        double splitValue { get; set; }
        int splitDimension { get; set; }

        #region Constructors
        public Data()
        {
        }
        public Data(List<double[]> data)
        {
            PointsCoordinates = data;
            CreateBoundBox();
            CalculateBoundBox();

        }
        #endregion

        #region Bounding Box
        void CalculateBoundBox()
        {
            /*
             *    Method calculate bounding box for current points collection
             */

             // Finding min and max points value in each dimention to get bounding box
            for (int i = 0; i < dimensionsCount; i++)
            {
                foreach (var p in PointsCoordinates)
                {
                    BoundingBoxMin[i] = (p[i] < BoundingBoxMin[i]) ? p[i] : BoundingBoxMin[i];
                    BoundingBoxMax[i] = (p[i] > BoundingBoxMax[i]) ? p[i] : BoundingBoxMax[i];
                }
            }

            // Calculate bounding box lengths(max - min, value) of each dimention
            for (int i = 0; i < dimensionsCount; i++)
            {
                BoundingBoxLength[i] = BoundingBoxMax[i] - BoundingBoxMin[i] + 1;
            }
        }

        void CreateBoundBox()
        {
            /*
             *    Method create and initialize bounding box properties 
             */
            BoundingBoxMin = new List<double>();
            BoundingBoxMax = new List<double>();
            BoundingBoxLength = new List<double>();

            for (int i = 0; i < dimensionsCount; i++)
            {
                BoundingBoxMin.Add(double.PositiveInfinity);
                BoundingBoxMax.Add(double.NegativeInfinity);
                BoundingBoxLength.Add(0);
            }
        }
        #endregion

        #region Divide data
        public void DividePoint(out Data leftData, out Data rightData)
        {
            /*
             *    Alghoritm:
             *    1) Choose a random dimension proportional to l(i) / ( l(1) + ... + l(n))
             *       where l(i) = max x∈S xi − minx∈Sxi.
             *      
             *       Doing by <GetRandomDimension> method.
             *      
             *    2) Choose Xi ∼ Uniform[minx∈S xi, maxx∈S xi]
             *      
             *       Doing by <RandomTools.RandomDouble> method.
             *       
             *    3) Split points between two groups: leftPoints, rightPoint
             *       and than update leftData and rightData with this points group
             *    
             */
            List<double[]> leftPoints;
            List<double[]> rightPoints;

            // Choice dimension and check correction
            do
            {
                splitDimension = GetRandomDimension();

            } while (!IsCorrectRandomDimension(splitDimension));


            // cicle keep split points while both groups not empty
            do
            {
                leftPoints = new List<double[]>();
                rightPoints = new List<double[]>();
                splitValue = RandomTools.RandomDouble(BoundingBoxMin[splitDimension],
                                                      BoundingBoxMax[splitDimension]);

                foreach (var point in PointsCoordinates)
                {
                    if (point[splitDimension] < splitValue)
                    {
                        leftPoints.Add(point);
                    }
                    else
                    {
                        rightPoints.Add(point);
                    }
                }
                //Console.WriteLine(ToString());
            } while ((leftPoints.Count == 0) || (rightPoints.Count == 0));

            // create and update data
            leftData = new Data(leftPoints);
            rightData = new Data(rightPoints);
        }

        int GetRandomDimension()
        {
            /*
             *   Method return number of dimension, that was choice random proportional to it length
             *   
             *   l1 - lenght of bounding box in 1st dimention
             *   l2 - lenght of bounding box in 2nd dimention
             *   ...
             *   l1 / lTotal - chance(weight) to choice 1st dimension
             *   l2 / lTotal - chance(weight) to choice 2nd dimension
             *   ...
             *   
             *   lTotal = l1 + l2 + ... 
             *   
             */

            var weights = new List<decimal>();
            var dimensionsNumbers = new List<int>();

            // calculate total length of bounding box(lTotal)
            decimal totalBoundLength = DoubleTools.Sum(BoundingBoxLength);

            // calculate waights
            for (int i = 0; i < dimensionsCount; i++)
            {
                weights.Add(DoubleTools.Div(BoundingBoxLength[i], totalBoundLength));
            }

            // fill collection with dimension indexs
            for (int i = 0; i < dimensionsCount; i++)
            {
                dimensionsNumbers.Add(i);
            }

            return RandomTools.Choice(dimensionsNumbers, weights);
        }

        bool IsCorrectRandomDimension(int randDim)
        {
            /*
             *   This method check can we split current point in this dimention or no.
             *   If points in dimension <randDim> has same coordinat than we can't split them -> return false
             *   Otherwise thay has different coordinat -> he can split them -> return true
             */
            double[] firstPoint = PointsCoordinates[0];
            foreach (var point in PointsCoordinates)
            {
                if(firstPoint[randDim] != point[randDim])
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Show
        public override string ToString()
        {
            var str = new StringBuilder();

            foreach (var p in PointsCoordinates)
            {
                str.Append("[");
                foreach (var c in p)
                {
                    str.Append(c + " ");
                }
                str.Append("]");
            }
            str.Append($"\tdim: {splitDimension}  splt: {splitValue}");
            return str.ToString();
        }
        #endregion
    }
}
