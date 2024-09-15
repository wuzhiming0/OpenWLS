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
using System.Windows.Interop;
using OpenWLS.Server.LogDataFile.LAS;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Client.Requests;
using OpenWLS.Server.LogDataFile.Models.NMRecord;
using OpenWLS.Server.LogDataFile.Models;


namespace OpenWLS.Client.LogDataFile
{
    /// <summary>
    /// Interaction logic for Exploere.xaml
    /// </summary>
    public partial class Explore : UserControl
    {
        DataFileInfor dfi;
        public DataFileInfor DataFileInfor {
            get { return dfi; }
            set
            {
                dfi = value;
                ncList.NCItems = dfi.NMRecords;
                chDetails.Measurements = dfi.Measurements;
            }
        }
        public int SampleTop { get; set;  }
   /* 
        public Measurements Measurements
        {
            get { return chDetails.Measurements; }
            set
            {
                chDetails.Measurements = value;
            }
        }

    public NMRecords NMRecords
        {
            set
            {
                ncList.NCItems = value;
            }
        }
    */
        public Explore()
        {
            InitializeComponent();
            chDetails.DataListBtn = datBtn;
            datBtn.IsEnabled = false;     
         //   viewBtn.Content = FindResource("ViewIcon"); 
        }
        /*
        public string Proc1D(DataReader r)
        {
            return c1dLV.ProcData(r);
        }


        public string ProcXD(DataReader r)
        {
            return cxdLV.ProcData(r);
        }


        public string ProcNc(DataReader r)
        {
            return ncCntl.ProcData(r);
        }
        */
        public void HideDetails()
        {
            c1dLV.Visibility = System.Windows.Visibility.Hidden;
            cxdLV.Visibility = System.Windows.Visibility.Hidden;
            chEditor.Visibility = System.Windows.Visibility.Hidden;
            frameEditor.Visibility = System.Windows.Visibility.Hidden;
            ncCntl.Visibility = System.Windows.Visibility.Hidden;
        }

        private void datBtn_Click(object sender, RoutedEventArgs e)
        {
       
          List<Measurement> chs = chDetails.Measurements.Where(h => h.Selected).ToList();   
            string str = chs[0].Id.ToString();
            HideDetails();
            if (!chs[0].Head.NumberType)
            {
               // c1dLV.Init(chDetails.SelectedItems);
               // LdfRequest.Get1DNumberData(dfi.FileName, , c1dLV);
            }

            else
            {
                if (chs[0].Head.SampleElements > 1)
                {

                    //  ArGui.GetRequest(ArProc.str_req_ch_data_xd + "\n0\n" + cxdLV.SamplesView.ToString() + "\n" + ch.ID.ToString());
                }
                else
                {
                    c1dLV.Init(chs); int[] ids = new int[chs.Count];
                    for (int i = 0; i < chs.Count; i++)
                        ids[i] = chs[i].Id;
                    LdfRequest.Get1DNumberData(dfi.FileName, ids, c1dLV);
                }
            }

        }

        private void datBtn_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            datBtn.Content = datBtn.IsEnabled ? FindResource("ListData_On") : FindResource("ListData_Off");
        }

        private void ncList_NcItemDbClicked(object sender, EventArgs e)
        {
            NMRecord a = (NMRecord)sender;
         //   ArGui.SendGetRequest(ArProc.str_req_nc_detail + "\n" + ArGui.FullFileName
         //       + "\n" + a.ID.ToString() + "\n" + ((int)a.Type).ToString()    );

            HideDetails();
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            //foreach (MHeadC ch in chDetails.MHeads)
            datBtn.IsEnabled = false;
        }

        private void chDetails_ItemSelected(object sender, EventArgs e)
        {
            HideDetails();
            if(sender is OpenWLS.Server.LogDataFile.Models.Frame)
            {
                frameEditor.DataContext = sender;
                frameEditor.Visibility = Visibility.Visible;
            }
            if (sender is Measurement)
            {
                chEditor.DataContext = ((Measurement)sender);
                chEditor.updateBtn.Visibility = Visibility.Hidden;
                chEditor.Visibility = Visibility.Visible;
            }
        }

        void ExportLas(LasVersion v, string ch_names)
        {
         //   if (MessageBox.Show("Export file\n" + ArGui.FullFileName + "\n toLAS", "Export LAS", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
         //       ArGui.SendRequest(ArProc.str_req_export_las + "\n" + ((int)v).ToString() + "\n" + ArGui.FullFileName + "\n" + ch_names); 
        }

        string GetSelectedChNames()
        {
            string str=null;
            foreach(Measurement ch in chDetails.Measurements)
            {
                if (ch.Selected)
                    str = str + "," + ch.Head.LongName;
            }
            if (str == null)
                return null;
            return str.Substring(1, str.Length-1);
        }
        public void ExportLasV2( )
        {
            string ch_names = GetSelectedChNames();
            if (ch_names != null)
                ExportLas(LasVersion.V20, ch_names);
        }
        public void ExportLasV3()
        {
            string ch_names = GetSelectedChNames();
            if(ch_names != null)
                ExportLas(LasVersion.V30, ch_names);
        }

        public void ExportDlis()
        {

        }

    }
}
