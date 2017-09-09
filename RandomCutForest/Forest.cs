using RandomCutForest.Components;
using RandomCutForest.Additional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Collections.Concurrent;

namespace RandomCutForest
{
     /// <summary>
     ///  This class represent Random Cut Forest.
     ///  Build forest and than you can find anomaly score from new point.
     /// </summary>
    public class Forest
    {

        /// <summary>
        /// Class for transfering data into threads pool for anomaly calculation
        /// </summary>
        class StateAnomaly
        {
            public CountdownEvent countEvent;
            public Tree tree;
            public decimal[] point;

            public StateAnomaly(CountdownEvent countEvent, Tree tree, decimal[] point)
            {
                this.countEvent = countEvent;
                this.tree = tree;
                this.point = point;
            }
        }

        /// <summary>
        /// Class for transfering data into thread pool for build tree
        /// </summary>
        class StateTree
        {
            public CountdownEvent countEvent;
            public List<decimal[]> points;

            public StateTree(CountdownEvent countEvent, List<decimal[]> points)
            {
                this.countEvent = countEvent;
                this.points = points;
            }
        }


        #region Vars

        public int TreeCount { get; }
        public double SamplingRatio { get; }

        public int MaxPointsInTree { get; private set; }
        int pointsPass;

        BlockingCollection<Tree> treesCollection = new BlockingCollection<Tree>();

        object lockAnomaly = new object();
        decimal tmpAnomalScore;

        #endregion

        #region Constructor

        /// <summary>
        ///   Initialize forest with some paremetrs.
        /// </summary>
        /// <param name="treeCount">Tree numbers in forest</param>
        /// <param name="samplingRatio"> Sampling ratio</param>
        public Forest(int treeCount, double samplingRatio)
        {
            TreeCount = treeCount;
            SamplingRatio = samplingRatio;
        }

        #endregion

        #region Anomaly score
        /// <summary>
        ///   Method to find anomaly score of point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns>Anomaly score</returns>
        public decimal PointAnomaly(decimal[] point)
        {
            /*
             *      Calculate anomaly in all trees and return average
             */
            tmpAnomalScore = 0;
            var pool = new CountdownEvent(TreeCount);

            foreach (var tree in treesCollection)
            {
                ThreadPool.QueueUserWorkItem(Anomaly, new StateAnomaly(pool, tree, point));
            }

            pool.Wait();
            pool.Dispose();

            pointsPass++;
            return tmpAnomalScore / TreeCount;
        }

        /// <summary>
        /// Method that might be in thread, calculate point anomaly in some tree
        /// </summary>
        /// <param name="state">All data that need to find anomaly</param>
        void Anomaly(object state)
        {
            var st = state as StateAnomaly;
            var point = st.point;
            var tree = st.tree;

            var pointData = new Data(new List<decimal[]> { point });
            var pointNode = new Node(pointData);

            //calculate anomaly score(CoDisp)
            tree.Add(pointNode);
            var coDisp = tree.CoDisp(pointNode);

            // check need add this point in tree or no
            // insertPosition is number of element to replace
            int insertPosit = Vitter(pointsPass);
            if (insertPosit != -1)
            {
                var deleteNode = tree.Find(tree.RootNode.Value.GetPoint(insertPosit));
                tree.Delete(deleteNode);
            }
            else
            {
                tree.Delete(pointNode);
            }

            lock (lockAnomaly)
            {
                tmpAnomalScore += coDisp;
            }

            // send that thread complite
            st.countEvent.Signal();
        }

        #endregion

        #region Build forest

        /// <summary>
        /// Build forest from start data.
        /// </summary>
        /// <param name="feedData">Start data to build forest</param>
        public void BuildForest(List<decimal[]> feedData)
        {
            Data.DimensionsCount = (byte)(feedData.First().Count());
            MaxPointsInTree = (int)(feedData.Count * SamplingRatio);

            var pool = new CountdownEvent(TreeCount);
            for (int i = 0; i < TreeCount; i++)
                ThreadPool.QueueUserWorkItem(MakeTree, new StateTree(pool, feedData));

            pool.Wait();
            pool.Dispose();

            pointsPass = feedData.Count;
        }

        /// <summary>
        /// Method that might be in thread, make samplitng of data and build tree.
        /// </summary>
        /// <param name="state">All data that need to build tree</param>
        void MakeTree(object state)
        {
            var st = state as StateTree;
            var points = st.points;

            var n = 0;
            var tmp = new List<decimal[]>();
            for (int i = 0; i < MaxPointsInTree; i++)
                tmp.Add(null);

            foreach (var point in points)
            {
                int insertPosit = Vitter(n);
                if (insertPosit != -1)
                    tmp[insertPosit] = point;
                n++;
            }
            var tree = new Tree(new Data(tmp));
            tree.MakeTree();

            // add tree in list
            treesCollection.Add(tree);

            st.countEvent.Signal();
        }

        #endregion

        #region Vitter sampling

        /// <summary>
        /// Method that make reservoir sampling.
        /// </summary>
        /// <param name="number">Number of current element</param>
        /// <returns></returns>
        int Vitter(int number)
        {
            if (number < MaxPointsInTree)
                return number;
            var r = RandomTools.RandomInt(0, number);
            if (r < MaxPointsInTree)
                return r;
            else
                return -1;
        }

        #endregion

        /// <summary>
        /// Test method that print all trees in forest.
        /// </summary>
        public void Show()
        {
            foreach (var t in treesCollection)
            {
                Console.WriteLine(t);
            }
        }
    }
}
