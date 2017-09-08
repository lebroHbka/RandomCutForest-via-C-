using RandomCutForest.Components;
using RandomCutForest.Additional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RandomCutForest
{
    public class Forest
    {
        class State
        {
            /*
             *      Class for transfering data into threads pool
             */
            public CountdownEvent countEvent;
            public Tree tree;
            public decimal[] point;

            public State(CountdownEvent countEvent, Tree tree, decimal[] point)
            {
                this.countEvent = countEvent;
                this.tree = tree;
                this.point = point;
            }
        }
        
        #region Vars

        public int TreeCount { get; }
        public double SamplingRatio { get; }

        int pointsMaxCount;
        int pointsPass;

        List<Tree> treesList = new List<Tree>();

        object key = new object();
        decimal tmpAnomalScore;

        #endregion

        #region Constructor

        public Forest(int treeCount, double samplingRatio)
        {
            TreeCount = treeCount;
            SamplingRatio = samplingRatio;
        }

        #endregion

        #region Anomaly score

        public decimal PointAnomaly(decimal[] point)
        {
            /*
             *      Calculate anomaly in all trees and return average
             */
            tmpAnomalScore = 0;
            var pool = new CountdownEvent(TreeCount);

            for (int i = 0; i < TreeCount; i++)
            {
                ThreadPool.QueueUserWorkItem(Anomaly, new State(pool, treesList[i], point));
            }

            pool.Wait();
            pool.Dispose();

            pointsPass++;
            return tmpAnomalScore / TreeCount;
        }

        void Anomaly(object state)
        {
            /*
             *      Calculate point anomaly in some tree
             *      All data in state
             */
            var st = state as State;
            var point = st.point;
            var tree = st.tree;

            var pointData = new Data(new List<decimal[]> { point });
            var pointNode = new Node(pointData);

            //calculate anomaly score(CoDisp)
            tree.Add(pointNode);
            var coDisp = tree.CoDisp(pointNode);

            // check need add this point in tree or no
            // insertPosition is number of element to replace
            int insertPosit;
            lock (key)
            {
                insertPosit = Vitter(pointsPass);
                tmpAnomalScore += coDisp;
            }

            if (insertPosit != -1)
            {
                var deleteValue = tree.RootNode.Value.PointsList[insertPosit];
                var deleteNode = tree.Find(new Data(new List<decimal[]> { deleteValue }));
                tree.Delete(deleteNode);
            }
            else
            {
                tree.Delete(pointNode);
            }

            // send that thread complite
            st.countEvent.Signal();
        }
        #endregion

        #region Build forest

        public void BuildForest(List<decimal[]> feedData)
        {
            Data.DimensionsCount = (byte)(feedData.First().Count());
            pointsMaxCount = (int)(feedData.Count * SamplingRatio);

            for (int i = 0; i < TreeCount; i++)
                MakeTree(feedData);
            pointsPass = feedData.Count;
        }

        void MakeTree(List<decimal[]> points)
        {
            var n = 0;
            var tmp = new List<decimal[]>();
            for (int i = 0; i < pointsMaxCount; i++)
                tmp.Add(null);

            foreach (var point in points)
            {
                var insertPosit = Vitter(n);
                if (insertPosit != -1)
                    tmp[insertPosit] = point;
                n++;
            }
            var tree = new Tree(new Data(tmp));
            tree.MakeTree();

            // add tree in list
            treesList.Add(tree);
        }

        #endregion

        #region Vitter sampling

        int Vitter(int number)
        {
            if (number < pointsMaxCount)
                return number;
            var r = RandomTools.RandomInt(0, number);
            if (r < pointsMaxCount)
                return r;
            else
                return -1;
        }

        #endregion

        public void Show()
        {
            foreach (var t in treesList)
            {
                Console.WriteLine(t);
                Console.WriteLine("==========================");
            }
        }
    }
}
