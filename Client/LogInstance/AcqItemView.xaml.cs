using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.IO;
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
using OpenWLS.Client.LogInstance.Instrument;
using OpenWLS.Client.GView.Models;
using OpenWLS.Client.LogDataFile;
using OpenWLS.Client.Requests;
using OpenWLS.Server.LogInstance.OperationDocument;
using OpenWLS.Server.DBase.Models.GlobalDb;
//using OpenWLS.Server.LogInstance;

namespace OpenWLS.Client.LogInstance
{
    /// <summary>
    /// Interaction logic for AcqItemList.xaml
    /// </summary>
    public partial class AcqItemView : UserControl
    {
        public event EventHandler CheckChanged;

        AcqItems acqItems;


        public AcqItems AcqItems
        {
            set {  
                acqItems = value;
                actDg.ItemsSource = null;
                actDg.ItemsSource = acqItems;
                ICollectionView view = CollectionViewSource.GetDefaultView(actDg.ItemsSource);
                view.GroupDescriptions.Clear();
                PropertyGroupDescription groupDescription = new PropertyGroupDescription("IId");
                view.GroupDescriptions.Add(groupDescription);
            }
            get {  return acqItems;   }
        }

        bool enableEdit;
        public bool EnableEdit   { get { return enableEdit; }
            set
            {
                enableEdit = value;
                //     acqList.EnableEdit(enableEdit);
                if (enableEdit)
                {
                    editPanel.Visibility = Visibility.Visible;
                    editPanelLabel.Visibility = Visibility.Visible;
                }
                else
                {
                    editPanel.Visibility = Visibility.Collapsed;
                    editPanelLabel.Visibility = Visibility.Collapsed;
                }
            }
        }

        public AcqItemView()
        {
            InitializeComponent();
      //      acqItems = new Server.LogInstance.OperationDocument.AcqItems();
            actDg.SelectionChanged += Act_SelectionChanged;
            CheckChanged += Act_CheckChanged;
        }

        private void Act_CheckChanged(object sender, EventArgs e)
        {
            AcqItem acq = (AcqItem)actDg.SelectedItem;

        }

        private void Act_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          //  btnGrid.IsEnabled = acqList.actDg.SelectedItem != null;
        }


        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (actDg.SelectedItem != null)
            {
                AcqItem acq = (AcqItem)actDg.SelectedItem;
          //      string str = "Are you sure to update data group " + acq.DataGroup.ToString() + " of tool " + acq.ToolName;
          //      if (MessageBox.Show(str, "update acquisition item", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
          //          api.Client.SendRequest("ACT\n" + LogInstance.str_api_act_req_act_update + "\n" + JsonConvert.SerializeObject(acq));
            }
        }
        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (actDg.SelectedItem != null)
            {
                AcqItem acq = (AcqItem)actDg.SelectedItem;
        //        string str = "Are you sure to delete data group " + acq.DataGroup.ToString() + " of tool " + acq.ToolName;
        //        if (MessageBox.Show(str, "Delete acquisition item", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        //            api.Client.SendRequest("ACT\n" + LogInstance.str_api_act_req_act_del + "\n" + JsonConvert.SerializeObject(acq));
            }
        }


        

        /*
        public void EnableEdit(bool b)
        {
            //   GridView gv = actListView.View as GridView;
            //   gv.Columns[1].Width = editAct ? widthColEnable : 0;
            //   gv.Columns[3].Width = editAct ? 70 : 0;

            actDg.Columns[3].IsReadOnly = !b;
            actDg.Columns[2].IsReadOnly = !b;
        }*/

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (CheckChanged != null)
                CheckChanged("1", e);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (CheckChanged != null)
                CheckChanged("0", e);
        }

        void AddMeasurement(string str_m, InstrumentC? instc, AcqItem acqItem,  OperationDocument doc)
        {
            MeasurmentParas mps = new MeasurmentParas(str_m);
            MeasurementOd m = new MeasurementOd();
            if (mps.DbId != null)
            {
                MeasurementDb? m_mdb = MeasurementRequest.GetById((int)mps.DbId).Result;
                if (m_mdb != null)
                    m.CopyFrom(m_mdb);
            }
            if (mps.Name != null)
            {
                MeasurementDb? m_mdb = MeasurementRequest.GetByName((string)mps.Name).Result;
                if (m_mdb != null)
                    m.CopyFrom(m_mdb);
            }

            if (mps.MPoint != null) m.MPoint = (double)mps.MPoint;
            if (mps.NuDisp != null) m.NuDisp = (bool)mps.NuDisp;
            if (mps.Record != null) m.Record = (bool)mps.Record;            
            //Get sub ID if model exists
            if (mps.SubModel != null)
            {
                m.SubModel = mps.SubModel;
                InstSub? s = doc.DhTools.Subs.Where(a=>a.Model == m.SubModel).LastOrDefault();
                if(s != null)
                    m.SubId = s.Id;
            }
            else
            {
                // if downhole tool set last sub id
                if (instc != null && instc.SurfaceEqu == null)
                    m.SubId = doc.DhTools.Subs.Last().Id;
            }

            if (mps.Id != null) m.Id = (int)mps.Id;
            
            MeasurementOds ms = doc.Measurements;
            m.Id = ms.GetNxtId();
            m.AcqId = acqItem.Id;
            if (instc != null)
                m.IId = (int)instc.Id;
            ms.Add(m);
            acqItem.Measurements.Add(m);
        }

        public void RemoveAcqItemsOFInstrument(int iid, MeasurementOds ms)
        {
            List<AcqItem> ais = AcqItems.Where(a=>a.IId == iid).ToList();
            foreach(AcqItem ai in ais)
            {
                acqItems.Remove(ai);
                foreach(MeasurementOd m in ai.Measurements)
                    ms.Remove(m);
            }
        }
        public void InitAcqItems(MeasurementOds ms, InstrumentCs insts)
        {
            foreach (AcqItem a in acqItems)
            {
                a.Measurements = new MeasurementOds();
                foreach (int mid in a.MIds)
                    a.Measurements.Add(ms.Where(m => m.Id == mid).FirstOrDefault());
                InstrumentOd? inst = insts.Where(i => i.Id == a.IId).FirstOrDefault();
                if (inst != null)
                    a.InstName = inst.Name;
            }
            AcqItems = acqItems;
        }
        public void AddInstumentAcqItems(InstrumentC instc, OperationDocument doc)
        {
            List<MGroup> mgs = MGroupRequest.GetMGroupsOfInst(instc.DbId).Result;
            if (mgs == null) return;
            foreach (MGroup g in mgs)
            {
                AcqItem acqItem = new(g)
                {
                    IId = instc.Id,
                    Id = acqItems.Count == 0 ? 1 : acqItems.Max(a => a.Id) + 1,
                    InstName = instc.Name
                };
                acqItem.Measurements = new MeasurementOds();
                acqItems.Add(acqItem);
                AddMeasurement("Name=DEPT", null,  acqItem, doc);
                AddMeasurement("Name=TIME", null,  acqItem, doc);
                foreach (string s in g.Ms.Split('\n'))
                    AddMeasurement(s, instc,  acqItem, doc);
                acqItem.MIds = acqItem.Measurements.Select(m => m.Id).ToArray();
            }
            AcqItems = acqItems;
        }

    }
}
