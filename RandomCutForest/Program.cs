using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RandomCutForest.src;

namespace RandomCutForest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static event UpdateShedule AddPoints;
        public delegate void UpdateShedule(int x, double y);


        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var f = new FormWithResults();

            var forest = new Forest();
            forest.MakeForest();
            f.Points = forest.GetRealData();
            f.Anomaly = forest.GetAnomalyData();

            //f.Points = (new Reciver()).GetFeedData();
            

            Application.Run(f);
        }



        
    }
}
