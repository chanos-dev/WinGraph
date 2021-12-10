using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Sample
{
    public partial class Form1 : Form
    {
        private Thread ProcessThread { get; set; }

        private double[] CPUArray = new double[60];

        private double[] MemoryArray = new double[60];
        private Chart[] Charts { get; set; }

        public Form1()
        {
            InitializeComponent();
            InitializeChart();
            Run();
        }

        private void Run()
        {
            ProcessThread = new Thread(() => GetPerformanceCounters());
            ProcessThread.IsBackground = true;
            ProcessThread.Start();
        }

        private void InitializeChart()
        {
            Charts = new[]
            {
                chart1,
                chart2,
                chart3,
                chart4,
                chart5,
                chart6,
                chart7,
                chart8,
                chart9,
            };

            foreach(var chart in Charts)
            {                
                chart.Series.Clear();
                chart.Legends.Clear();

                #region Serial 박스
                var legend = new Legend("legend");

                legend.BackColor = Color.Black;
                legend.ForeColor = Color.Yellow;
                chart.Legends.Add(legend);
                #endregion

                #region 타이틀
                var title = new Title("title");
                title.Text = chart.Name;
                title.ForeColor = Color.Yellow;

                chart.Titles.Add(title);

                #endregion
                var serialo = new Series("one");
                //serialo.Color = Color.Blue;
                serialo.ChartType = SeriesChartType.Spline;
                serialo.ChartArea = chart.ChartAreas.First().Name;

                var serialw = new Series("two");
                //serialo.Color = Color.Yellow;
                serialw.ChartType = SeriesChartType.Spline;
                serialw.ChartArea = chart.ChartAreas.First().Name;


                chart.Series.Add(serialo);
                chart.Series.Add(serialw);

                chart.ChartAreas[0].BackColor = chart.BackColor = Color.Black;

                chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LimeGreen;
                chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LimeGreen;

                chart.ChartAreas[0].AxisX.LineColor = Color.Yellow;
                chart.ChartAreas[0].AxisY.LineColor = Color.Yellow;

                chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.Yellow;
                chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.Yellow;
            } 
        }

        private async void GetPerformanceCounters()
        {
            var cpuPerfCounter = new PerformanceCounter("Processor Information", "% Processor Time", "_Total", true);

            var memoryCounter = new PerformanceCounter("Memory", "Available KBytes", true);

            while (true)
            {
                #region CPU 사용량
                CPUArray[CPUArray.Length - 1] = Math.Round(cpuPerfCounter.NextValue(), 0);
                Array.Copy(CPUArray, 1, CPUArray, 0, CPUArray.Length - 1);
                #endregion

                #region 메모리 사용량 
                using (ManagementClass mc = new ManagementClass("Win32_OperatingSystem")) 
                { 
                    using (ManagementObject o = mc.GetInstances().Cast<ManagementObject>().FirstOrDefault()) 
                    { 
                        float physicalMemorySize = float.Parse(o["TotalVisibleMemorySize"].ToString()); 
                        float rate = ((physicalMemorySize - memoryCounter.NextValue()) / physicalMemorySize) * 100;  

                        MemoryArray[MemoryArray.Length - 1] = Math.Min(100f, Math.Max(0f, rate));
                        Array.Copy(MemoryArray, 1, MemoryArray, 0, MemoryArray.Length - 1);
                    } 
                }

                #endregion

                UpdateCpuChart(Charts.Take(5));
                UpdateMemoryChart(Charts.Skip(5));

                await Task.Delay(100);
            }
        }

        private void UpdateCpuChart(IEnumerable<Chart> charts)
        {
            if (this.InvokeRequired)
            {                   
                this.Invoke(new Action(() => UpdateCpuChart(charts)));
            }
            else
            {
                foreach (var chart in charts)
                {
                    chart.Series["one"].Points.Clear();
                    chart.Series["two"].Points.Clear();

                    for (int i = 0; i < CPUArray.Length; i++)
                    {
                        chart.Series["one"].Points.AddY(CPUArray[i]);
                        chart.Series["two"].Points.AddY(CPUArray[i] + int.Parse(chart.Name.Replace("chart", "")));
                    }
                }
            }
        } 

        private void UpdateMemoryChart(IEnumerable<Chart> charts)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateMemoryChart(charts)));
            }
            else
            {
                foreach (var chart in charts)
                {
                    chart.Series["one"].Points.Clear();
                    chart.Series["two"].Points.Clear();

                    for (int i = 0; i < MemoryArray.Length; i++)
                    {
                        chart.Series["one"].Points.AddY(MemoryArray[i]);
                        chart.Series["two"].Points.AddY(MemoryArray[i] - int.Parse(chart.Name.Replace("chart", "")));
                    } 
                }
            } 
        }

        private void palette변경ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new Form2())
            {
                form.StartPosition = FormStartPosition.CenterParent;
                form.ShowDialog();

                foreach(var chart in Charts)
                    chart.Palette = form.SelectedChartColorPalette;
            } 
        } 
    }
}