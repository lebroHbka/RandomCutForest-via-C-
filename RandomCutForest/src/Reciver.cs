using RandomCutForest.src.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomCutForest.src
{
    delegate void NewData(List<decimal[]> newData);
    class Reciver
    {
        string feedData = @"C:\Users\Vit\Documents\Visual Studio 2017\Projects\RandomCutForest\RandomCutForest\src\Information\1.csv";
        string realData = @"C:\Users\Vit\Documents\Visual Studio 2017\Projects\RandomCutForest\RandomCutForest\src\Information\2.csv";

        public event NewData GetNewData;

        public Reciver()
        {
        }

        int k = 3;


        List<decimal[]> Parser(string file)
        {
            var points = new List<decimal[]>();
            using (StreamReader sr = new StreamReader(file))
            {
                while (!sr.EndOfStream)
                {
                    var str = sr.ReadLine();
                    if (str != "")
                    {
                        var p = str.Split(',');
                        p[0] = p[0].Replace('.', ',');
                        p[1] = p[1].Replace('.', ',');


                        points.Add(new decimal[] { decimal.Parse(p[0]),
                                                   decimal.Parse(p[1])
                                                 });
                    }
                }
            }
            return points;
        }


        public void Send(List<decimal[]> newData)
        {
            GetNewData.Invoke(newData);
        }


        public List<decimal[]> GetFeedData()
        {
            return MakeNDimensions(Parser(feedData), k);
        }

        public List<decimal[]> GetRealData()
        {
            return MakeNDimensions(Parser(realData),k);
        }

        public List<decimal[]> GetTimeStamp()
        {
            var d = Parser(realData);
            return d.GetRange(0, d.Count - k + 1);
        }

        public List<decimal[]> MakeNDimensions(List<decimal[]> points, int dim)
        {
            var rez = new List<decimal[]>();

            for (int i = 0; i < points.Count - dim + 1; i++)
            {
                decimal[] p = new decimal[dim];
                for (int j = i; j < i + dim; j++)
                {
                    p[j - i] = points[j][1];
                }
                rez.Add(p);
            }
            return rez;
        }

    }
}
