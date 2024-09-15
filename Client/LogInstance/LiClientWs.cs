using OpenWLS.Client.LogInstance.Instrument;
using OpenWLS.Client.Dock;
using OpenWLS.Client.GView.GUI;
using OpenWLS.Client.Requests;
using OpenWLS.Server.LogInstance;
using OpenWLS.Server.LogInstance.OperationDocument;
using OpenWLS.Server.Base;
using OpenWLS.Server.DBase.Models.GlobalDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using AcqItems = OpenWLS.Server.LogInstance.OperationDocument.AcqItems;
using System.Net.Sockets;
using System.Windows;
using System.Threading;
using OpenWLS.Server.WebSocket;
using Client;
using OpenWLS.Server.GView.ViewDefinition;
using System.Windows.Controls;

namespace OpenWLS.Client.LogInstance
{
    public partial class LiClientMainCntl : IWsMsgProc
    {
        public static LiClientMainCntl? GetLiClientMainCntl()
        {
            return ((MainWindow)App.Current.MainWindow).LiClientMain;
        }
        IEdgeDeviceC? edgeDeviceC;
        EdgeDeviceCntl? edgeDeviceCntl;
        ClientWebSocket? webSocket;
        public bool WsClosed { get { return webSocket == null; } }
     //   public N1dDisplay N1DDisplay { get; set; }  
        public int Id { get; set; }

        public VdDFile RtVdFile { get; set; }

        public void StartWs()
        {
            Task.Run(() => WsRequest.StartWs(this));
            while(webSocket == null)
                Thread.Sleep(100);
            wsTxBuffer = new WsTxBuffer(webSocket);
            SendConnectRequest();
          //  await WsRequest.StartWs(this);
        }
        void ProcessEdgeDevMsg(DataReader r)
        {
            int s = r.ReadUInt16();
            byte[]? bs = s == 0 ? null : r.ReadByteArray(s);
            Dispatcher.BeginInvoke(() =>
            {
                if (edgeDeviceCntl != null)
                    edgeDeviceCntl.ProcessEdgeDeviceMsg(bs);
            });
        }
        void ProcessInstMsg(DataReader r)
        {
            Dispatcher.BeginInvoke(() =>
            {
                int iid = r.ReadInt32();
                InstrumentC? inst = insts_all.Find(x => x.Id == iid);
                if (inst != null)
                    inst.ProcInstMsg(r);
            });
        }

        public void ProcessWsRxMsg(DataReader r)
        {
            ushort msg_type = r.ReadUInt16();
            switch(msg_type)
            {
                case (ushort)LiWsMsg.UpdateGView:
                    ProcUpdateGView(r);
                    break;
                case (ushort)LiWsMsg.Inst:
                    ProcessInstMsg(r);
                    break;
                case (ushort)LiWsMsg.EdgeDev:
                    ProcessEdgeDevMsg(r);
                    break;
                case (ushort)LiWsMsg.LiState:
                    ProcLogInstanceState(r);
                    break;                
                case (ushort)LiWsMsg.LogInstance:
                    ProcLogInstance(r);
                    break;
                case (ushort)LiWsMsg.OperationDoc:
                    ProcOpDocument(r);
                    break;
                case (ushort)LiWsMsg.NewGView:
                    ProcNewGView(r);
                    break;
                case (ushort)LiWsMsg.RemoveGView:
                    ProcRemoveGView(r);
                    break;
            }
        }
        public void RequestEnd()
        {

        }

        public void SetWebSocket(ClientWebSocket? ws)
        {
            webSocket = ws;
            if(ws == null)
            {
                ((MainWindow)Application.Current.MainWindow).CloseLogInstance(false);
            }
            else
            {
              
            }
        }

        void ProcLogInstanceState(DataReader r)
        {
            ushort state = r.ReadUInt16();
            ushort record = r.ReadUInt16();
            Dispatcher.BeginInvoke(() =>
            {
                LiState = (LiState)state;
                toolBar.Record = record != 0;
            });
        }
        void ProcLogInstance(DataReader r)
        {
            string json = r.ReadStringWithSizeInt32();
            Server.LogInstance.LogInstance? instance = Newtonsoft.Json.JsonConvert.DeserializeObject<Server.LogInstance.LogInstance>(json);
            if (instance == null)
            {
                ClientGlobals.SysLog.AddMessage($"Connect invalid Log Instance {Id}! disconnected", System.Windows.Media.Colors.Red);
                if(webSocket != null) webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    DockControl.IsVisible = true;
                });
                SendOperationDocumentRequest();
            }

        }

        void InitGViews(OperationDocument doc)
        {
            if (doc.VdDocs != null)
            {
                foreach (VdDocumentOd vd in doc.VdDocs)
                {
                    LogGView logGView = new LogGView();
                    VdDocumentRt vrt = (VdDocumentRt)VdDocument.FromJson(vd.Content, true);
                    vrt.Id = vd.Id;
                    logGView.VdDocument = vrt;
                    ((MainWindow)App.Current.MainWindow).AddDisplayDoc(logGView, true);
                    SendNewGViewRequest(vd);
                }
            }
        }



        void ProcOpDocument(DataReader r)
        {
            string json = r.ReadStringWithSizeInt32();
            OperationDocument? doc = Newtonsoft.Json.JsonConvert.DeserializeObject<OperationDocument>(json);
            if (doc == null)
            {
                ClientGlobals.SysLog.AddMessage($"Invalid operation document! disconnected", System.Windows.Media.Colors.Red);
                if (webSocket != null) webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    SetOperationDocument(doc);
                    InitGViews(doc);
                    ((MainWindow)App.Current.MainWindow).N1dDisplay.UpdateDisplay(this, doc.Measurements.Where(a => a.GetTotalElements() == 1).ToList(), insts_all);
                });
            }
        }



        void ProcGViewMsg(int id, string msg)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (id < 0)
                {
                    ClientGlobals.SysLog.AddMessage($"Failed to {msg} GView {-id}.", 0xffff0000);
                    return;
                }
                LogGView? logGView = ((MainWindow)App.Current.MainWindow).RtGViews.Where(a => a.Id == id).FirstOrDefault();
                if (logGView == null)
                {
                    ClientGlobals.SysLog.AddMessage($"Failed to {msg} GView {id}.", 0xffff0000);
                    return;
                }
              //  ((VdDocumentRt)logGView.VdDocument).Connected = true;
                msg = StringConverter.FirstCharToUpperLinq(msg);
                ClientGlobals.SysLog.AppendMessage($"{msg}d GView {id}.");
            });
        }
        void ProcNewGView(DataReader r)
        {
            int id = r.ReadInt32();
            ProcGViewMsg(id, "create");
        }
        void ProcUpdateGView(DataReader r)
        {
            int id = r.ReadInt32();
            ProcGViewMsg(id, "update");
        }
                    
        void  ProcRemoveGView(DataReader r)
        {
            int id = r.ReadInt32();
            ProcGViewMsg(id, "remove");
        }

    }
}
