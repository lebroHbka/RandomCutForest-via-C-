using RandomCutForest.src.Components;
using RandomCutForest.src.Additional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace RandomCutForest.src
{
    class Forest
    {

        // config
        public byte DimensionCount { get; }
        public int TreeCount { get; }
        public double SamplingRatio { get; }
        public int StartSampleSize { get; }

        int pointsMaxCount;
        int pointsPass;

        Reciver reciver;

        List<Tree> treesList = new List<Tree>();
        List<decimal[]> trainingData;
        List<decimal[]> realData;
        List<decimal[]> timeStamp;

        public Forest()
        {
            TreeCount = Int32.Parse(ConfigurationManager.AppSettings["TreeCount"]);
            SamplingRatio = Double.Parse(ConfigurationManager.AppSettings["SamplingRatio"]);

            reciver = new Reciver();



            trainingData = reciver.GetFeedData();
            realData = reciver.GetRealData();
            timeStamp = reciver.GetTimeStamp();
            DimensionCount = (byte)realData.First().Length;


            Data.DimensionsCount = DimensionCount;

            pointsMaxCount = (int)(trainingData.Count * SamplingRatio);

        }

        public List<decimal[]> GetAnomalyData()
        {

            var rez = new List<decimal[]>();

            for (int i = 0; i < realData.Count; i++)
            {
                var anomalyScore = PointAnomaly(realData[i]);
                rez.Add(new decimal[] { timeStamp[i][0], anomalyScore });

                pointsPass++;
            }
            return rez;
        }

        public List<decimal[]> GetRealData()
        {
            var rez = new List<decimal[]>();
            for (int i = 0; i < realData.Count; i++)
            {
                rez.Add(new decimal[] { timeStamp[i][0], realData[i][0] });
            }
            return rez;
        }


        public decimal PointAnomaly(decimal[] point)
        {
            decimal anomalyScore = 0;

            for (int i = 0; i < TreeCount; i++)
            {
                var pointData = new Data(new List<decimal[]> { point });
                var pointNode = new Node(pointData);
                var tree = treesList[i];

                //calculate anomaly score
                //var disp = tree.Complexity();
                //tree.Add(d);
                //disp = tree.Complexity() - disp;
                //anomalyScore += disp;

                tree.Add(pointNode);
                anomalyScore += tree.CoDisp(pointNode);


                // check need add this point in tree or no
                // insertPosition is number of element to replace
                var insertPosit = Vitter(pointsPass);
                if (insertPosit != -1)
                {
                    var deleteValue = treesList[i].RootNode.Value.PointsList[insertPosit];

                    var deleteNode = tree.Find(new Data(new List<decimal[]> { deleteValue }));

                    tree.Delete(deleteNode);

                }
                else
                {
                    tree.Delete(pointNode);
                }
            }
            return anomalyScore / TreeCount;
        }


        #region Make forest

        public void MakeForest()
        {
            for (int i = 0; i < TreeCount; i++)
                MakeTree(trainingData);
            pointsPass = trainingData.Count;
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
