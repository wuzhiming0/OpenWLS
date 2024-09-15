using Newtonsoft.Json;
using OpenWLS.Client.LogInstance;
using OpenWLS.Client.LogInstance.Instrument;
using OpenWLS.Client.Dock;
using OpenWLS.Client.Requests;
using OpenWLS.Server.LogInstance;
using OpenWLS.Server.LogInstance.OperationDocument;
using OpenWLS.Server.DBase.Models.GlobalDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

//using OpenWLS.Server.LogInstance.OperationDocument;
using OpenWLS.Client.GView.Models;
using OpenWLS.Server.GView.ViewDefinition;
using System.Net.WebSockets;
using System.Threading;
using OpenWLS.Server.LogInstance.Edge;
//using System.Windows.Forms;

namespace OpenWLS.Client.LogInstance
{
    /// <summary>
    /// Interaction logic for LiClientMainCntl.xaml
    /// </summary>
    public partial class LiClientMainCntl : UserControl, IDockableCntl
    //,  IWsMsgProc
    {
        InstrumentCs insts_all;
        List<InstrumentDb> instruments_db;
        OperationControlFile ocf;
        OperationDocument opDoc;
        LiMenuStatus menuStatus;
        LiState liState;

        AcqItemView acqView;
        PanelsView subView;
        bool update_lock;
        public DockControl DockControl { get; set; }
        public InstrumentCs Instruments { get { return insts_all; } }
        public LiMenuStatus MenuStatus
        {
            get { return menuStatus; }
        }
        public LogToolBar ToolBar { get { return toolBar; } }
        public LiState LiState
        {
            get { return liState; }
            set {
                if(liState == value ) return ;
                liState = value;
                switch(liState)
                {
                    case LiState.Edit:
                        ocfNameTb.Visibility = Visibility.Visible;
                        toolBar.Visibility = Visibility.Collapsed;
                        masterInstBtn.Visibility = Visibility.Visible;
                        toolDepotBtn.Visibility = Visibility.Visible;
                        instruments_db = InstrumentRequest.GetAll().Result;
                        break;
                    case LiState.Playback:
                    case LiState.Playback_Standby:
                        ocfNameTb.Visibility = Visibility.Collapsed;
                        toolBar.Visibility = Visibility.Visible;
                        masterInstBtn.Visibility = Visibility.Collapsed;
                        toolDepotBtn.Visibility = Visibility.Collapsed;
                        toolBar.State = liState;
                        if (edgeDeviceCntl == null || !(edgeDeviceCntl is EdgeDevicePlaybackCntl) )
                            edgeDeviceCntl = new EdgeDevicePlaybackCntl();
                        break;
                    default: 
                        ocfNameTb.Visibility = Visibility.Collapsed;
                        toolBar.Visibility = Visibility.Visible;
                        masterInstBtn.Visibility = Visibility.Collapsed;
                        toolDepotBtn.Visibility = Visibility.Collapsed;
                        toolBar.State = liState;
                        break;

                }

                menuStatus.SetApiState(liState);
                ClientGlobals.SysLog.AddMessage($"LogInstance State = {liState}");
               // if (liState == LiState.Log_EdgeDeviceConnected)

            }
        }

        public OperationControlFile OCF { 
            get 
            { 
                ocf.Name = ocfNameTb.Text;
               // ocf.Body = JsonConvert.SerializeObject(GetOperationDocument());
                return ocf; 
            }
            set
            {
                ocf = value;
                if (value == null) return;

                ocfNameTb.Text = ocf.Name;
                if(ocf.Body != null)
                    SetOperationDocument(Newtonsoft.Json.JsonConvert.DeserializeObject<OperationDocument>(ocf.Body));
            }
        }
/*
        public VdMeasurements GetVdMeasurements()
        {
            return new VdMeasurements(opDoc.Measurements, RtVdFile);
        }
*/
        public InstrumentCs? DhTools
        {
            get { 
                if (toolList.ItemsSource == null) return null;
                else return (InstrumentCs)toolList.ItemsSource;
            }
        }
        public InstrumentCs? SfEquipments
        {
            get { 
                if (equList.ItemsSource == null) return null;
                else return (InstrumentCs)equList.ItemsSource;
            }
        }
        public MeasurementDisplays MeasurementDisplays
        {
            get
            {
                MeasurementDisplays ds = new();
                ds.AddDisplays(DhTools);
                ds.AddDisplays(SfEquipments);
                return ds;
            }
        }


            
        void AddSubs(InstrumentGroup inst_group, InstrumentC instc)
        {
            string[] ss = instc.SubDbIds.Split(',');
            foreach (string s in ss)
            {
                int sdb_id = Convert.ToInt32(s);
                InstSubDb? sub_db = InstSubRequest.GetInstSubAsync(sdb_id).Result;
                if (sub_db != null)
                {
                    InstSub sub = new()
                    {
                        Id = opDoc.GetNxtSubId(),
                        IId = instc.Id,
                    };
                    sub.CopyFrom(sub_db);
                    inst_group.Subs.Add(sub);   
               //     instc.SubIds.Add(sub.Id);
                }
            }
        }


        private void LiClientMainCntl_IntrumentDpDbClicked(object sender, EventArgs e)
        {
            InstrumentDb inst = (InstrumentDb)sender;
            InstrumentC? instc = InstrumentC.CreateInstDb(inst);
            if (instc == null) return;
            InstrumentGroup ig = opDoc.DhTools;
            ListBox listBox = toolList;
            if (instc.SurfaceEqu != null)
            {
                ig = opDoc.SfEquipment;
                listBox = equList;
            }

            if (instc is IEdgeDeviceC)
            {
                foreach (InstrumentC equ in equList.ItemsSource)
                {
                    equ.Close();
                    opDoc.Measurements.RemoveMeasurementOfInst(equ.Id);
                    opDoc.ACT.RemoveAcqItemsOfInst(equ.Id);

                }
                ig = new InstrumentGroup();
                opDoc.SfEquipment = ig;
                equList.ItemsSource = new InstrumentCs();
            }

            instc.Id = opDoc.GetNxtInstId();
            InstrumentCs insts = (InstrumentCs)listBox.ItemsSource;
            insts.Add(instc);
            listBox.ItemsSource = null;
            listBox.ItemsSource = insts;

            AddSubs(ig, instc);
            acqView.AddInstumentAcqItems(instc, opDoc);
            MainWindow mw = (MainWindow)Application.Current.MainWindow;
            instc.CreateCntl();
            foreach (MeasurementDisplay md in instc.Displays)
                mw.AddMDisplayCntl(md);
            ig.Insts.Add(instc);
        }


        public LiClientMainCntl()
        {
            InitializeComponent();
            ocf = new OperationControlFile();
            opDoc = new OperationDocument();
            menuStatus = new LiMenuStatus();
            acqView = new AcqItemView();
            toolList.ItemsSource = new InstrumentCs();
            equList.ItemsSource = new InstrumentCs();
            update_lock = false;
            SetOperationDocument(opDoc);
        }




        public FrameworkElement GetControl(string name)
        {
            if(name == "LogToolBar")
                return toolBar;
            return null;
        }

        void SetGridCntl(FrameworkElement cntl)
        {
            itemGrid.Children.Clear();
            itemGrid.Children.Add(cntl);
        }


        private void ActBtn_Click(object sender, RoutedEventArgs e)
        {
            update_lock = true;
            SetGridCntl(acqView);
            toolList.SelectedItem = null;
            equList.SelectedItem = null;
            update_lock = false;
        }
        private void equBtnClick(object sender, RoutedEventArgs e)
        {
            if (subView == null)
            {
                subView = new PanelsView()
                {
                    EditMode = liState == LiState.Edit ? SubEditMode.all :
                    liState == LiState.Log || liState == LiState.CV || liState == LiState.Playback ? SubEditMode.none : SubEditMode.asset,
                    Pannels = opDoc.SfEquipment.Subs
                };

                if (liState >= LiState.Log_EdgeDeviceConnected && liState < LiState.Playback_NoEdge && edgeDeviceCntl != null)
                {
                    subView.edgeDevGd.Children.Add(edgeDeviceCntl);
                }
            }
            SetGridCntl(subView);
        }
        private void toolBtnClick(object sender, RoutedEventArgs e)
        {
            ToolSubsView subView = new ToolSubsView();
            subView.Init(this);
     //       subView.Subs = opDoc.DhTools.Subs;
            subView.EnableEdit = liState == LiState.Edit;
            SetGridCntl(subView);
        }



        public void DeleteTool(int id, ListBox listBox, InstrumentGroup ig)
        {
            InstrumentCs insts = (InstrumentCs)listBox.ItemsSource;
            InstrumentC? inst = insts.Where(x => x.Id == id).FirstOrDefault();
            if (inst != null)
            {
                inst.Close();
                insts.Remove(inst);
                ig.DeleteInst(inst);
            }
            opDoc.ACT.RemoveAcqItemsOfInst(id);
            opDoc.Measurements.RemoveMeasurementOfInst(id);

            listBox.ItemsSource = null;
            listBox.ItemsSource = insts;
            acqView.AcqItems = opDoc.ACT;
        }
        private void toolDepotBtnClick(object sender, RoutedEventArgs e)
        {
            InstrumentList instList = new InstrumentList();
            instList.Instruments = instruments_db.Where(a=>a.SurfaceEqu == null).ToList();
            instList.IntrumentDbClicked += LiClientMainCntl_IntrumentDpDbClicked;
            SetGridCntl(instList);
        }

        private void masterInstBtnClickBtn_Click(object sender, RoutedEventArgs e)
        {
            InstrumentList instList = new InstrumentList();
            
            instList.Instruments = instruments_db.Where(a => a is IEdgeDevice ).ToList();
            instList.IntrumentDbClicked += LiClientMainCntl_IntrumentDpDbClicked;
            SetGridCntl(instList);
        }

        private void toolBar_StateChanged(object sender, EventArgs e)
        {
          /*  LiState s = (LiState)sender;
            if (menuStatus != null)
                menuStatus.SetApiState(s);
            editGrid.Visibility = s == LiState.Edit ? Visibility.Visible : Visibility.Hidden;
            if (s == LiState.Blank)
                ((APIClient)gui.Client).CloseOCF();*/
        }

        private void toolList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(toolList != null && (!update_lock))
            {
                update_lock = true;
                InstrumentC instc = (InstrumentC)toolList.SelectedItem;
                if(instc != null) SetGridCntl(instc.Cntl);
                equList.SelectedItem = null;
                update_lock = false;
            }
        }
        public void CalcMPoints()
        {
            opDoc.DhTools.Subs.CalcMPoints();
            opDoc.Measurements.SetMPoint(opDoc.DhTools.Subs);
        }
        public OperationDocument GetOperationDocument()
        {
            return opDoc;
        }

        
        void SetOperationDocument(OperationDocument? doc)
        {
            opDoc = doc;
            if (opDoc != null)
            {
                toolList.ItemsSource = null;
                toolList.ItemsSource = new InstrumentCs(opDoc.DhTools);
                equList.ItemsSource = null;
                equList.ItemsSource = new InstrumentCs(opDoc.SfEquipment);
                insts_all = new InstrumentCs();
                foreach (InstrumentC inst in toolList.ItemsSource)
                {
                    inst.InitInst(this);
                    insts_all.Add(inst);
                }
                foreach (InstrumentC inst in equList.ItemsSource)
                {
                    inst.InitInst(this);
                    insts_all.Add(inst);
                }
                var v  = insts_all.Where(a => a is IEdgeDeviceC).FirstOrDefault();
                if (v != null)
                {
                    edgeDeviceC = (IEdgeDeviceC)v;
                    edgeDeviceCntl = edgeDeviceC.GetEdgeDeviceCntl();
                }
                foreach (InstSub s in opDoc.DhTools.Subs)
                    s.Instrument = opDoc.DhTools.Insts.Where(a => a.Id == s.IId).FirstOrDefault();
                foreach (InstSub s in opDoc.SfEquipment.Subs)
                    s.Instrument = opDoc.SfEquipment.Insts.Where(a => a.Id == s.IId).FirstOrDefault();
                acqView.AcqItems = doc.ACT;
                acqView.InitAcqItems(doc.Measurements, insts_all);
                RtVdFile = new VdDFile()
                {
                    Name = "Realtime",
                    Measurements = new VdMeasurements(doc.Measurements, RtVdFile),
                };
                MainWindow mw = (MainWindow)Application.Current.MainWindow;
                foreach (MeasurementDisplay d in MeasurementDisplays)
                    mw.AddMDisplayCntl(d);
            }
            /*    Subs = subView.Subs.GetOperationDocumentSubs(),
                    Tools = GetOpDocInsts(),
                    ACT = acqView.AcqItems,
                    Measurements = measurements,
            */
        }


        public void Close()
        {
            foreach (InstrumentC inst in toolList.Items)
                inst.Close();
            foreach (InstrumentC inst in equList.Items)
                inst.Close();
            if (webSocket != null)
            {
                WebSocket ws = webSocket;
                webSocket = null;
                if(ws.State == WebSocketState.Open)
                    ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);            
            }
        }

        private void equList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (equList != null && (!update_lock))
            {
                update_lock = true;
                InstrumentC instc = (InstrumentC)equList.SelectedItem;
                if (instc != null) SetGridCntl(instc.Cntl);
                toolList.SelectedItem = null;
                update_lock = false;
            }
        }
        /*
public void ProcessWsRxMsg(byte[] rx_buf, int count)
{

}

public void RequestEnd()
{

}
*/
    }
}
