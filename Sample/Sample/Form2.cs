using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Sample
{
    public partial class Form2 : Form
    {
        public ChartColorPalette SelectedChartColorPalette { get; set; }
        public Form2()
        {
            InitializeComponent();
            InitializeControl();
        }

        private void InitializeControl()
        {
            comboBox1.Items.AddRange(Enum.GetNames(typeof(ChartColorPalette)));
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Enum.TryParse(comboBox1.Text, out ChartColorPalette palette))
            {
                SelectedChartColorPalette = palette;
            }
            else
            {
                SelectedChartColorPalette = ChartColorPalette.None;
            }

            DialogResult = DialogResult.OK;
        }
    }
}
