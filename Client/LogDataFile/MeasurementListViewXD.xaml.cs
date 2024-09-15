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

using System.Data;
using System.Collections;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile;

namespace OpenWLS.Client.LogDataFile
{
    /// <summary>
    /// Interaction logic for ChDatdListViewxD.xaml
    /// </summary>
    public partial class MeasurementListViewXD : UserControl
    {
        int sbPre;
        delegate void UpdateDataDelegate();
        float[][] buffer;
        DataTable dt;
        public int SamplesTotal { get; set; }
        public int SamplesView { get; set; }
        public int TopSampleView { get; set; }

        public MeasurementListViewXD()
        {
            InitializeComponent();
            dt = new DataTable();
        }

        public void UpdateDataList()
        {
            int c = dt.Columns.Count;
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(new UpdateDataDelegate(UpdateDataList), null);
            else
            {
                dt.Rows.Clear();
                int k = 0;
                float[] fs = buffer[sbPre];
                while (k < fs.Length)
                {
                    DataRow dr = dt.NewRow();
                    object[] os = new object[c];
                    for (int j = 0; j < c; j++)
                        os[j] = fs[k++];
                    dr.ItemArray = os;
                    dt.Rows.Add(dr);
                }
                datDg.ItemsSource = null;
                datDg.ItemsSource = dt.AsDataView();
            }
        }
/*
        public string ProcData(DataReader r)
        {
            int section = r.ReadInt32();
            float[] fs = new float[s];
            for (int i = 0; i < s; i++)
                fs[i] = r.ReadSingle();

            buffer[section] = fs;
            if (section == 0)
                UpdateDataList();
            return null;
        }
*/
        public void Init(Measurement m)
        {
            dt.Clear();
            dt.Columns.Clear();
            dt.Columns.Add("Index", typeof(float));

            SamplesTotal = m.Samples;
            TopSampleView = 0;

            //        SamplesView = ArProc.Get1dViewSamples(items.Count, SamplesTotal);

            int k = 1;
            if (SamplesView == SamplesTotal)
                scBar.Visibility = System.Windows.Visibility.Hidden;
            else
            {
                k = SamplesTotal / SamplesView;
                if (SamplesTotal > k * SamplesView)
                    k++;
                scBar.Maximum = k - 1;
                scBar.SmallChange = 1;
                scBar.Value = 0;
                sbPre = 0;
                scBar.Visibility = System.Windows.Visibility.Visible;
            }

            buffer = new float[k][];
            //       datDg.ItemsSource = dt.AsDataView();
        }
 
        private void scBar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            int k = (int)e.NewValue;
            if (k != sbPre)
            {
                sbPre = k;
                UpdateDataList();
            }

        }


    }
}
