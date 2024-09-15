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
using OpenWLS.Server.LogDataFile.LAS;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.Base;
using System.Threading.Channels;
using OpenWLS.Client.Requests;
using OpenWLS.Server.WebSocket;
using OpenWLS.Server.LogDataFile.Models;
using System.Net.WebSockets;


namespace OpenWLS.Client.LogDataFile
{
    /// <summary>
    /// Interaction logic for MeasurementListView1D.xaml
    /// </summary>
    public partial class MeasurementListView1D : UserControl, IWsMsgProc
    {
        public const int i_req_1d_package_size = 1024;
        int sbPre;

//        string chIDs;
        Measurements ms;
        string[] formats;
        DataTable dt;
        int scrollValue;
        int scrollValueOld;
        public int SamplesTotal { get; set; }
        public int SamplesView { get; set; }
        public int TopSampleView { get; set; }
        double[][] ds;
        public MeasurementListView1D()
        {
            InitializeComponent();
            dt = new DataTable();
            //ms = new ();
            scrollValueOld = 0;
        }

        public void UpdateDataList()
        {
           // int c = dt.Columns.Count;
            Dispatcher.Invoke(new Action(() =>
            {
                dt.Rows.Clear();
                try
                {
                    int k = Math.Min(i_req_1d_package_size, ms[0].Samples);
                    // int k = ms[0].Head.NumberType ? ((float[][])ms[0].Head.Tag)[scrollValue].Length : ((string[][])ms[0].Head.Tag)[scrollValue].Length;
                    int c = ms.Count;
                    int offset = scrollValue * i_req_1d_package_size;
                    for (int i = 0; i < k; i++)
                    {
                        DataRow dr = dt.NewRow();
                        object[] os = new object[c+1];
                        os[0] = offset++;
                        for (int j = 0; j < c; j++)
                        {
                            if (ms[j].Head.NumberType)
                            {
                                var d = ds[j][i];
                                if (formats[j] == null)
                                    os[j + 1] = d.ToString();
                                else
                                {
                                    if (formats[j][0] == '{')
                                        os[j + 1] = string.Format(formats[j], d);
                                    else
                                        os[j + 1] = d.ToString(formats[j]);
                                }
                            }
                            //  else
                            //      os[j + 1] = ((string[][])ms[j].Head.Tag)[scrollValue][i];
                        }
                        dr.ItemArray = os;
                        dt.Rows.Add(dr);
                    }
                }
                catch (Exception e)
                {

                }
                datDg.ItemsSource = null;
                datDg.ItemsSource = dt.AsDataView();
                Visibility = System.Windows.Visibility.Visible;
                //    this.
            }));
        }
    
        public void Init(IList ms_all)
        {
            ms = new Measurements();

            foreach (Measurement m in ms_all)
                ms.Add(m);
            formats = new string[ms.Count];
            dt.Clear();
            dt.Columns.Clear();
            dt.Columns.Add("Index", typeof(int));
            scrollValue = 0;
            int k = 0;
            foreach (Measurement m in ms)
            {
                if (m.Head.NumberType)
                    formats[k] = m.GetStdNumericFormat();
                dt.Columns.Add(m.Head.Name, typeof(string));
                k++;
            }

            SamplesTotal = ms[0].Samples;
            TopSampleView = 0;

            SamplesView = SamplesTotal > i_req_1d_package_size ? i_req_1d_package_size : SamplesTotal;
            k = 1;
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
            Visibility = Visibility.Visible;
        }

        void ProcessMvSection(DataReader r)
        {
            int mid = r.ReadInt32();
            int bid = r.ReadInt32();  // block
            int offset = r.ReadInt32(); //offset
            ushort s = r.ReadUInt16();
            byte[] bs = r.ReadByteArray(s);
            Measurement? m = ms.Where(m => m.Id == mid).First();
            if(m != null)
            {
                MVBlock? b = m.MVBlocks.GetBlock(bid);
                if (b != null)
                {
                    if (b.VBuffer == null)
                        b.VBuffer = new byte[m.Head.GetSampleBytes() * b.Samples];
                    Buffer.BlockCopy(bs, 0, b.VBuffer, offset, bs.Length);
                }
            }
        }
        public void ProcessWsRxMsg(DataReader r)
        {
            ushort msg_type = r.ReadUInt16();
            switch( msg_type )
            {
              //  case WsService.response_ldf_mv_blocks:
              //      ProcessMvBlocks(r);
              //      break;
                case WsService.response_ldf_mv_section:
                    ProcessMvSection(r);
                    break;
            }
        }
        public void RequestEnd()
        {
            int k = Math.Min(i_req_1d_package_size, ms[0].Samples);
            // int k = ms[0].Head.NumberType ? ((float[][])ms[0].Head.Tag)[scrollValue].Length : ((string[][])ms[0].Head.Tag)[scrollValue].Length;
            int c = ms.Count;

            ds = new double[c][];
            for ( int i = 0; i < c; i++ )
            {
                MVReader r = new MVReader(ms[i]);
                ds[i] = r.Read1DDoubles(0, ms[i].Samples, 0, false);
            }
            UpdateDataList();
            ClientGlobals.SysLog.AddMessage(new Server.DBase.Models.LocalDb.SyslogItem()
            {
                Color = 0xff000000,
                Msg = "data request completed",
                Time = DateTime.Now.Ticks
            });
            //  }));
        }
     
        private void scBar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            scrollValue = (int)e.NewValue;
            Measurement m = ms[0];
            if(m.Head.NumberType)
            {
          //    if  ( ( (float[][])ch.Tag)[scrollValue] == null)
          //          ArGui.SendGetRequest(ArProc.str_req_ch_dat_1d + "\n" + ArGui.FullFileName + "\n" + scrollValue.ToString() + "\n" + chIDs); 
          //    else
                    UpdateDataList();
            }
            else
            {
           /*   
                if (((string[][])ms.Tag)[scrollValue] == null)
                {
                    if (scrollValue != scrollValueOld)
                    {
                        //ArGui.SendGetRequest(ArProc.str_req_ch_dat_1d + "\n" + ArGui.FullFileName + "\n" + scrollValue.ToString() + "\n" + chIDs);
                        scrollValueOld = scrollValue;
                   }
                }
                else
                    UpdateDataList();
           */
            }
     
        }
        public void SetWebSocket(ClientWebSocket ws)
        {
           // webSocket = ws;
        }



    }
}
