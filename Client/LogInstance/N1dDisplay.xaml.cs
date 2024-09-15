using System;
using System.Collections.Generic;
using System.ComponentModel;
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

using Newtonsoft.Json;
using OpenWLS.Client.Dock;
using OpenWLS.Client.LogInstance;
using OpenWLS.Client.LogInstance.Instrument;
using OpenWLS.Client.Requests;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.LogInstance;
using OpenWLS.Server.LogInstance.Instrument;
using OpenWLS.Server.LogInstance.OperationDocument;

namespace OpenWLS.Client.LogInstance
{
    /// <summary>
    /// Interaction logic for N1dDisplay.xaml
    /// </summary>
    public partial class N1dDisplay : UserControl, IDockableCntl
    //, IWsMsgProc
    {
        public event EventHandler ItemAdded;
        public event EventHandler ItemDeleted;
        //   DataChannelSummary dataChannels;
        List<MeasurementOd> m1ds;
        N1dItems n1ds;
        LiClientMainCntl liClientMainCntl;
        /*
        public List<MeasurementOd> Measurments
        {
            get
            {
                return m1ds;
            }
        }*/
        public DockControl DockControl { get; set; }
        public N1dDisplay()
        {
            InitializeComponent();
        }

        public void UpdateDisplay(LiClientMainCntl liClientMainCntl,  List<MeasurementOd> ms, InstrumentCs insts)
        {
            Dispatcher.BeginInvoke(() =>
            {
                m1ds = ms;
                this.liClientMainCntl = liClientMainCntl;
                n1ds = new N1dItems(m1ds, insts);
                ICollectionView view = CollectionViewSource.GetDefaultView(n1ds);
                view.GroupDescriptions.Clear();
                PropertyGroupDescription groupDescription = new PropertyGroupDescription("InstName");
                view.GroupDescriptions.Add(groupDescription);

                n1dActive.ItemsSource = n1ds;
                chsLV.ItemsSource = m1ds.Where(a => a.NuDisp == null).ToList();
                liClientMainCntl.SendWsPackage(GetNewN1dRequestBytes());
            });
        }

        public byte[] GetNewN1dRequestBytes()
        {
            int c = 6 + (n1ds.Count << 2);
            DataWriter w = new DataWriter(c);
            w.WriteData((ushort)LiWsMsg.NewMv1Ds);
            w.WriteData(n1ds.Count);
            foreach (N1DItem i in n1ds)
                w.WriteData(i.Id);
            return w.GetBuffer();
        }

        void UpdateVal(Measurement1DVal mv)
        {

        }

        public void ProcVals(DataReader r)
        {
            string json = r.ReadStringWithSizeInt32();
            List<Measurement1DVal>? vs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Measurement1DVal>>(json);
            if (vs != null)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    foreach (Measurement1DVal mv in vs)
                        UpdateVal(mv);
                });
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            chsLV.Visibility = chsLV.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            if (chsLV.Visibility != Visibility.Visible)
                eleGrid.Visibility = Visibility.Hidden; 
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            if(n1dActive.SelectedItem != null)
            {
                N1DItem n1d = (N1DItem)n1dActive.SelectedItem;
                if (ItemDeleted != null)
                    ItemDeleted(n1d, new EventArgs());
/*
                n1ds.Remove(n1d);
                if (n1d.Element == -1)
                    dcsAvaiable.Add(new DataChannelBasic(n1d));
                UpdateDisplay();
*/
            }
            
        }

        private void chsLV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(chsLV.SelectedItem != null)
            {
           /*     MHead1 c = (MHead1)chsLV.SelectedItem;
                string d = c.;
                if(d != null && d != "1")
                {
                    eleGrid.Visibility = Visibility.Visible;
                    eleGrid.Tag = c; 
                    eleTb.Text = "0";
                }else
                {  
                    N1DItem n1d = new N1DItem(c, -1);              
                    if (ItemAdded != null)
                        ItemAdded(n1d, new EventArgs());
                } */
            }
          
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
          /*  DataChannelBasic ch = (DataChannelBasic)eleGrid.Tag;

            int d = Convert.ToInt32(eleTb.Text);
            N1DItem n1d = new N1DItem(ch, d);
            if (ItemAdded != null)
                ItemAdded(n1d, new EventArgs());            
            eleGrid.Visibility = Visibility.Hidden;
          */
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            eleGrid.Visibility = Visibility.Hidden;
        }

    }
}
