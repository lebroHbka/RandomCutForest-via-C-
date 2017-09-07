using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace RandomCutForest
{
    public partial class FormWithResults : Form
    {
        enum Coordinate
        {
            X = 0,
            Y = 1
        }

        public List<decimal[]> Points { get; set; }
        public List<decimal[]> Anomaly { get; set; }

        decimal minX = decimal.MaxValue;
        decimal maxX = decimal.MinValue;

        decimal minY = decimal.MaxValue;
        decimal maxY = decimal.MinValue;


        public FormWithResults()
        {
            InitializeComponent();
            Resize += ResizeHandler;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //DataSchedule.Width = Width;
            //DataSchedule.Height = Height / 2 + 20;
            foreach (var p in Points)
            {
                AddNewPoints(p[(int)Coordinate.X], p[(int)Coordinate.Y], DataSchedule, "Data_series");
            }
            foreach (var p in Anomaly)
            {
                AddNewPoints(p[(int)Coordinate.X], p[(int)Coordinate.Y], AnomalySchedule, "Anomaly_series");
            }
        }

        public void AddNewPoints(decimal x, decimal y, Chart chart, string series)
        {
            #region tmp
            //if (x < minX)
            //{
            //    chart1.ChartAreas[0].AxisX.Minimum = (double)(x - 1);
            //    minX = x;
            //}
            //if (x > maxX)
            //{
            //    chart1.ChartAreas[0].AxisX.Maximum = (double)(x + 1);
            //    maxX = x;
            //}

            //if (y < minY)
            //{
            //    chart1.ChartAreas[0].AxisY.Minimum = (double)(y - 1);
            //    minY = y;
            //}
            //if (y > maxY)
            //{
            //    chart1.ChartAreas[0].AxisY.Maximum = (double)(y + 1);
            //    maxY = y;
            //}
            #endregion   


            chart.ChartAreas[0].AxisY.IsStartedFromZero = false;
            //chart1.Series["test_data"].Points.RemoveAt(0);
            chart.Series[series].Points.AddXY(x, y);
            //chart1.ChartAreas[0].AxisX.Maximum = x;


        }  


        void ResizeHandler(object sender, EventArgs e)
        {
            DataSchedule.Width = Width;
            DataSchedule.Height = Height / 2 + 20;
        }

    }
}
