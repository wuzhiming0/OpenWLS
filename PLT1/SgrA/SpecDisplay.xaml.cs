using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;


using System.ComponentModel;
using System.Threading;
using System.IO;
using OpenWLS.Client.LogInstance.Instrument;

namespace OpenWLS.PLT1.SgrA
{
    /// <summary>
    /// Interaction logic for WaveDisplay.xaml
    /// </summary>
    public partial class SpecDisplay : MeasurementDisplay
    {
        G2dData d2Data;

        double[] xs;
        int selectChannel;

        public double MaxY { get { return chart1.ChartAreas[0].AxisY.Maximum; } }

        public int SelectChannel {
            get { return selectChannel; }

            set{
                selectChannel = value;
                chart1.Series[0].Points[0].XValue = selectChannel;
                chart1.Series[0].Points[1].XValue = selectChannel;
            }
        }

        void ResetXs(int size)
        {
            xs = new double[size];
            for (int i = 0; i < size; i++)
                xs[i] = i + 1;
        }

       public void UpdateDisplay()
       {
            if (d2Data.Updated)
            {
                if (xs.Length != d2Data.Data.Length)
                    ResetXs(d2Data.Data.Length);
                UpdateSpec();
                d2Data.Updated = false;
            }
       }

        public SpecDisplay()
        {
            InitializeComponent();

            chart1.MouseDown += chart1_MouseDown;
            chart1.MouseUp += chart1_MouseUp;
            chart1.MouseMove += chart1_MouseMove;

            ResetXs(256);
            d2Data = new G2dData();
            d2Data.SetData(new double[256]);

            AddSerial("cursorY", System.Drawing.Color.Gray);
            AddSerial("Spec", System.Drawing.Color.Blue);
            UpdateSpec();
        }


        public void UpdateData(double[] dat)
        {
            d2Data.SetData(dat);
        }

        void UpdateN2dList()
        {

            int c = d2Data.Data.Length;
            if (c != n2DList.Items.Count)
            {
                n2DList.Items.Clear();
                for (int i = 0; i < c; i++)
                    n2DList.Items.Add(i.ToString() + "---" + d2Data.Data[i].ToString() + "---" + d2Data.AccuDat[i].ToString());
            }
            else
            {
                for (int i = 0; i < c; i++)
                {
                    n2DList.Items[i] = i.ToString() + "---" + d2Data.Data[i].ToString() + "---" + d2Data.AccuDat[i].ToString();
                    if (i == selectChannel)
                        n2DList.SelectedItem = n2DList.Items[i];
                }
            }
        }

        void chart1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (chart1.Series.Count < 2)
                return;
            if (chart1.Series["cursorY"].Color != System.Drawing.Color.Red)
                return;
            try
            {
                double d = chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
                int x = (int)(d + 0.5);
                SelectChannel = x;
                n2DList.SelectedIndex = x;
            }
            catch (Exception e1)
            {

            }
        }

        void chart1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            chart1.Series["cursorY"].Color = System.Drawing.Color.Yellow;
        }

        void chart1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var pos = e.Location;

            var results = chart1.HitTest(pos.X, pos.Y, false,
                                            ChartElementType.DataPoint);
            foreach (var result in results)
            {
                if (result.ChartElementType == ChartElementType.DataPoint)
                {
                    var prop = result.Object as DataPoint;
                    if (prop != null)
                    {
                        var pointXPixel = result.ChartArea.AxisX.ValueToPixelPosition(prop.XValue);

                        // check if the cursor is really close to the point (2 pixels around the point)
                        if (Math.Abs(pos.X - pointXPixel) < 4 &&
                           ((prop.YValues[0] == -100) || (prop.YValues[0] == MaxY)))
                        {
                            chart1.Series["cursorY"].Color =  System.Drawing.Color.Red;
                        }
                    }
                }
            }
        }

        public void AddSerial(string strName, System.Drawing.Color color)
        {
            Series s = chart1.Series.Add(strName);
            chart1.Series[strName].Color = color;
            s.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            s.BorderWidth = 1;
            s.ShadowOffset = 1;
            s.BorderColor = System.Drawing.Color.FromArgb(180, 26, 59, 105);

            if (strName == "cursorY")
            {
                chart1.Series[strName].Points.Add(new DataPoint(100, -100));
                chart1.Series[strName].Points.Add(new DataPoint(100, 0));
                chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDotDot;
                chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDotDot;
                chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
                chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            }

        }

        delegate void UpdateSpecDelegate();
        void UpdateSpec()
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(new UpdateSpecDelegate(UpdateSpec), null);
            else
            {
                UpdateN2dList();
                int c = d2Data.AccuDat.Length;
                double  m = d2Data.AccuCntMax + 999;
                m = (int)(m / 1000);
                m = m * 1000;

                if(chart1.ChartAreas[0].AxisY.Maximum != m)
                    chart1.ChartAreas[0].AxisY.Maximum = m;

                chart1.Series.SuspendUpdates();
                chart1.Series[1].Points.Clear();
                for (int i = 0; i < xs.Length; i++ )
                    chart1.Series[1].Points.AddXY(xs[i], m-d2Data.AccuDat[i]);
                chart1.Series.ResumeUpdates();

                chart1.Series[1].Points[1].SetValueY(m); 
            }
        }

        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            d2Data.AccuCnt = 0;
        }


        private void n2DList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = n2DList.SelectedIndex;
            if (selectChannel != i)
            {
                selectChannel = i;
                SelectChannel = i;
            }
        }
        public int[] GetMeasurementIds()
        {
            return null;
        }
    }
}
