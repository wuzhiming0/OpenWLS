using Newtonsoft.Json;
using OpenWLS.Server.Base;
using OpenWLS.Server.DBase;
using OpenWLS.Server.GView.Models;
using OpenWLS.Server.GView.ViewDefinition;
using OpenWLS.Server.LogInstance;
using OpenWLS.Server.LogInstance.Instrument;
using OpenWLS.Server.LogInstance.Edge;
using OpenWLS.Server.LogInstance.OperationDocument;
using System;
using System.Diagnostics.Metrics;
using System.Net.WebSockets;
using System.Text;

namespace OpenWLS.Server.WebSocket
{
    /// <summary>
    /// GView: Id
    /// N1D:
    /// Inst: Inst Id, DisplayId
    /// </summary>
    // public enum ApinstanceWsMsgType { GView = 1, N1D = 2, Inst = 3 };
    [Flags]
    public enum ClientType { GView = 1, N1D = 2, Inst = 4 };
    public class LogInstanceWsClient : IDisposable, IGvStream
    {
        public ClientType ClientType { get; set; }
        public LogInstanceS LogInstance { get; set; }
        bool liOwner;
        public System.Net.WebSockets.WebSocket WebSocket { get; set; }

        public List<int> N1dMIds { get; set; }
        public List<int> NxdMIds { get; set; }

        public VdDocumentRts VdDocRts { get; set; }
        IServiceScopeFactory _scopeFactory;

        WsTxBuffer txBuffer;
        public LogInstanceWsClient(System.Net.WebSockets.WebSocket ws, int li_id, ClientType ct, IServiceScopeFactory scopeFactory)
        {
            N1dMIds = new();
            NxdMIds = new();
            VdDocRts = new();
            WebSocket = ws;
            ClientType = ct;
            txBuffer = new WsTxBuffer(ws);
            _scopeFactory = scopeFactory;
            LogInstanceS? li = ServerGlobals.logInstances.Where(a => a.Id == li_id).FirstOrDefault();
            if (li == null)
            {
                System.Diagnostics.Debug.WriteLine($"No LI {li.Id}!");
                ws.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            }
            else
            {
                liOwner = li.WsClients.Count == 0;
                LogInstance = li;
                li.WsClients.Add(this);
                SendTextMsg(li.GetJsonString(), LiWsMsg.LogInstance);
                System.Diagnostics.Debug.WriteLine($"Client {ws.ToString()} connected to LI {li.Id}.");
            }
        }

        public void Dispose()
        {
            if (WebSocket != null)
            {
                System.Diagnostics.Debug.WriteLine($"Client {WebSocket.ToString()} disconnected to LI {LogInstance.Id}.");
                WebSocket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            }
            LogInstance.WsClients.Remove(this);
            System.Diagnostics.Debug.WriteLine($"LI {LogInstance.Id} clients: {LogInstance.WsClients.Count}");
        }

        public void ProceRequest(DataReader r, int count)
        {
            ushort ws_msg_type = r.ReadUInt16();  //must have, do not remove
            ushort li_msg_type = r.ReadUInt16();
            switch (li_msg_type)
            {
                case (ushort)LiWsMsg.Inst:
                    ProcInstGuiMsg(r);
                    break;
                case (ushort)LiWsMsg.LiState:
                    ProcStateMsg(r);
                    break;
                case (ushort)LiWsMsg.CV:        // real time  
                    ProcCVMsg(r);
                    break;
        //        case (ushort)LiWsMsg.CVMan:     // post process, CV manager, move to API
        //            ProcCVManMsg(r);
        //            break;
                case (ushort)LiWsMsg.EdgeDev:
                    ProcEdgeDeviceMsg(r);
                    break;
                case (ushort)LiWsMsg.LogInstance:
                    SendTextMsg(LogInstance.GetJsonString(), LiWsMsg.LogInstance);
                    SendPackage(LogInstance.GetStateWsBytes());
                    SendPackage(LogInstance.GetEdgeDeviceWsBytes());
                    break;
                case (ushort)LiWsMsg.OperationDoc:
                    SendTextMsg(LogInstance.OpDoc.GetJsonString(), LiWsMsg.OperationDoc);
                    SendPackage(LogInstance.GetStateWsBytes());
                    SendPackage(LogInstance.GetEdgeDeviceWsBytes());
                    break;
                case (ushort)LiWsMsg.NewGView:
                    CreateNewGView(r);
                    break;
                case (ushort)LiWsMsg.UpdateGView:
                    UpdateGView(r);
                    break;
                case (ushort)LiWsMsg.RemoveGView:
                    RemoveGView(r);
                    break;
                case (ushort)LiWsMsg.Close:
                    CloseLogInstance(r);
                    break;
            }
        }

        void ProcStateMsg(DataReader r)
        {
            bool running = r.ReadUInt16() != 0;
            bool record = r.ReadUInt16() != 0;
            LogIndexMode lim = (LogIndexMode)r.ReadUInt16();
            if (running) LogInstance.StopLog();
            else         LogInstance.StartLog( lim, record);         
        }
        void ProcCVMsg(DataReader r)
        {

        }

        void CloseLogInstance(DataReader r)
        {
            int id = r.ReadInt32();
            if(LogInstance != null && id == LogInstance.Id)
                LogInstance.Close();
        }

        void ProcEdgeDeviceMsg(DataReader r)
        {
            if (LogInstance != null)
            {
                int s = r.ReadUInt16();
                byte[]? bs = s == 0 ? null : r.ReadByteArray(s);
                LogInstance.SendDlDevicePackage(bs);  // forward to edge 
            }
        }

        void ProcInstGuiMsg(DataReader r)
        {
            int iid = r.ReadInt32();
            Server.LogInstance.Instrument.Instrument? inst = LogInstance.Insts.Where(a=>a.Id == iid).FirstOrDefault();
            if (inst != null)
                inst.ProcGuiMsg(r);
        }

        #region GView
        void CreateNewGView(DataReader r) 
        {
            string json = r.ReadStringWithSizeInt32();
            VdDocumentOd? doc_od = Newtonsoft.Json.JsonConvert.DeserializeObject<VdDocumentOd>(json);
            if(doc_od != null)
            {
                VdDocumentRt doc_vd = (VdDocumentRt)VdDocument.FromJson(doc_od.Content, true);
                if(doc_vd != null)
                {
                    VdDocRts.Add(doc_vd);
                    SendMsg((ushort)LiWsMsg.NewGView, doc_od.Id);
                    return;
                }
            }
            SendMsg((ushort)LiWsMsg.NewGView, -doc_od.Id);
        }
        void RemoveGView(DataReader r)
        {
            int doc_id = r.ReadUInt16();
            VdDocumentRt? doc = VdDocRts.Where(a => a.Id == doc_id).FirstOrDefault();
            if (doc != null)
            {
                VdDocRts.Remove(doc);
                SendMsg((ushort)LiWsMsg.RemoveGView, doc_id);
                return;
            }
            SendMsg((ushort)LiWsMsg.RemoveGView, -doc_id);
        }
        void UpdateGView(DataReader r) 
        {
            string json = r.ReadStringWithSizeInt32();
            VdDocumentOd? doc_od = Newtonsoft.Json.JsonConvert.DeserializeObject<VdDocumentOd>(json);
            if(doc_od != null)
            {
                VdDocumentRt? doc_vd_new = Newtonsoft.Json.JsonConvert.DeserializeObject<VdDocumentRt>(doc_od.Content);
                if(doc_vd_new != null)
                {
                    VdDocumentRt? doc_vd_old = VdDocRts.Where(a=>a.Id == doc_vd_new.Id).FirstOrDefault();
                    if (doc_vd_old != null)
                    {
                        for(int i = 0; i < VdDocRts.Count; i++)
                        {
                            if (VdDocRts[i] == doc_vd_old)
                            {
                                VdDocRts[i] = doc_vd_new;
                                SendMsg((ushort)LiWsMsg.UpdateGView, doc_od.Id);
                                return;
                            }
                        }
                    }
                }
                SendMsg((ushort)LiWsMsg.UpdateGView, -doc_od.Id);
            }
            SendMsg((ushort)LiWsMsg.UpdateGView, 0);


        }
        #endregion


        void SendMsg(ushort ws_msg, int dat )
        {
            DataWriter w = new DataWriter(6);
            w.WriteData(ws_msg);
            w.WriteData(dat);
            txBuffer.WriteBuffer(w.GetBuffer());
        }

        public void SendTextMsg(string str, LiWsMsg msg_type)
        {
            byte[] bs = UTF8Encoding.UTF8.GetBytes(str);
            DataWriter w = new DataWriter(2 + 4 + bs.Length);
            w.WriteData((ushort)msg_type);
            w.WriteData(bs.Length);
            w.WriteData(bs);

            txBuffer.WriteBuffer(w.GetBuffer());
        }

        public void SendObject(IGvStreamObject ob, GvDocument doc)
        {
            byte[] bytes = ob.GetBytes(doc);
            txBuffer.WriteBuffer(bytes);
        }

        public void SendPackage(byte[]? bs)
        {
            if(bs != null)
                txBuffer.WriteBuffer(bs);
        }
    }

    public class LogInstanceWsClients : List<LogInstanceWsClient>
    {
 //       Timer updateTimer;
        public void SendPackage(ClientType ct, byte[] bs)
        {
            foreach (LogInstanceWsClient c in this)
                if((c.ClientType & ct) != 0)c.SendPackage(bs);
        }


        public void Close()
        {
            foreach (LogInstanceWsClient c in this)
                c.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            Clear();
        }
    }
}
