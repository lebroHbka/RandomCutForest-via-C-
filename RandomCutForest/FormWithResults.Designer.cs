namespace RandomCutForest
{
    partial class FormWithResults
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title3 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.DataSchedule = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.AnomalySchedule = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.DataSchedule)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AnomalySchedule)).BeginInit();
            this.SuspendLayout();
            // 
            // DataSchedule
            // 
            chartArea1.Name = "ChartArea1";
            this.DataSchedule.ChartAreas.Add(chartArea1);
            this.DataSchedule.Location = new System.Drawing.Point(0, 0);
            this.DataSchedule.Margin = new System.Windows.Forms.Padding(0);
            this.DataSchedule.Name = "DataSchedule";
            series1.BorderWidth = 2;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.LegendText = "Data";
            series1.MarkerSize = 8;
            series1.Name = "Data_series";
            this.DataSchedule.Series.Add(series1);
            this.DataSchedule.Size = new System.Drawing.Size(1000, 300);
            this.DataSchedule.TabIndex = 0;
            title1.BackImageWrapMode = System.Windows.Forms.DataVisualization.Charting.ChartImageWrapMode.Unscaled;
            title1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            title1.Name = "Title";
            title1.Text = "Data schedule";
            title2.Alignment = System.Drawing.ContentAlignment.BottomCenter;
            title2.BackImageAlignment = System.Windows.Forms.DataVisualization.Charting.ChartImageAlignmentStyle.Bottom;
            title2.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            title2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            title2.Name = "X";
            title2.Text = "Time";
            this.DataSchedule.Titles.Add(title1);
            this.DataSchedule.Titles.Add(title2);
            // 
            // AnomalySchedule
            // 
            chartArea2.Name = "ChartArea1";
            this.AnomalySchedule.ChartAreas.Add(chartArea2);
            this.AnomalySchedule.Location = new System.Drawing.Point(0, 260);
            this.AnomalySchedule.Margin = new System.Windows.Forms.Padding(0);
            this.AnomalySchedule.Name = "AnomalySchedule";
            series2.BorderWidth = 2;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series2.Name = "Anomaly_series";
            this.AnomalySchedule.Series.Add(series2);
            this.AnomalySchedule.Size = new System.Drawing.Size(1000, 300);
            this.AnomalySchedule.TabIndex = 1;
            title3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            title3.Name = "Anomaly Schedule";
            title3.Text = "Anomaly schedule";
            this.AnomalySchedule.Titles.Add(title3);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 561);
            this.Controls.Add(this.AnomalySchedule);
            this.Controls.Add(this.DataSchedule);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Random Cut Forest";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DataSchedule)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AnomalySchedule)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart DataSchedule;
        private System.Windows.Forms.DataVisualization.Charting.Chart AnomalySchedule;
    }
}

