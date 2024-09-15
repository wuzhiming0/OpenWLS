using OpenWLS.Server.Base;
using OpenWLS.Server.LogInstance;
using OpenWLS.Server.LogInstance.OperationDocument;
using OpenWLS.Server.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;

namespace OpenWLS.Client.LogInstance
{
    public partial class LiClientMainCntl
    {
        WsTxBuffer wsTxBuffer;
        public void SendConnectRequest()
        {
            DataWriter w = new DataWriter(12);
            w.WriteData(WsService.request_instance);
            w.WriteData((ushort)LiWsMsg.Connect);
            w.WriteData(Id);
            w.WriteData((UInt32)0xffffffff);            // client type : all
            wsTxBuffer.WriteBuffer(w.GetBuffer());
        }

        public void SendCloseLiRequest()
        {
            DataWriter w = new DataWriter(8);
            w.WriteData(WsService.request_instance);
            w.WriteData((ushort)LiWsMsg.Close);
            w.WriteData(Id);
            wsTxBuffer.WriteBuffer(w.GetBuffer());
        }

        public void SendOperationDocumentRequest()
        {
            DataWriter w = new DataWriter(4);
            w.WriteData(WsService.request_instance);
            w.WriteData((ushort)LiWsMsg.OperationDoc);
            wsTxBuffer.WriteBuffer(w.GetBuffer());
        }

        
        public void SendNewGViewRequest(VdDocumentOd doc)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(doc);
            SendTxtRequest(LiWsMsg.NewGView, json);
        }
        public void SendUpdateGViewRequest(VdDocumentOd doc)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(doc);
            SendTxtRequest(LiWsMsg.UpdateGView, json);
        }
        public void SendRemoveGViewRequest(int id)
        {
            DataWriter w = new DataWriter(8);
            w.WriteData(WsService.request_instance);
            w.WriteData((ushort)LiWsMsg.RemoveGView);
            w.WriteData(Id);
            wsTxBuffer.WriteBuffer(w.GetBuffer());
        }
        public void SendEdgeDeviceGuiPackage( byte[]? bs)
        {
            int c = 6;
            if (bs != null) c += bs.Length;
            DataWriter w = new DataWriter(c);
            w.WriteData(WsService.request_instance);    //2
            w.WriteData((ushort)LiWsMsg.EdgeDev);       //2
            if (bs == null)
                w.WriteData((ushort)0);                 //2
            else
            {
                w.WriteData((ushort)bs.Length);
                w.WriteData(bs);
            }
            wsTxBuffer.WriteBuffer(w.GetBuffer());
        }
        public void SendInstGuiPackage(int iid, ushort m_type, byte[]? bs)
        {
            int c = 12;
            if (bs != null) c += bs.Length;
            DataWriter w = new DataWriter(c);
            w.WriteData(WsService.request_instance);    //2
            w.WriteData((ushort)LiWsMsg.Inst);          //2
            w.WriteData(iid);                           //4
            w.WriteData(m_type);                        //2
            if (bs == null)
                w.WriteData((ushort)0);                 //2
            else
            {
                w.WriteData((ushort)bs.Length);
                w.WriteData(bs);
            }
            wsTxBuffer.WriteBuffer(w.GetBuffer());
        }
        public void SendWsPackage(byte[] bs)
        {
            wsTxBuffer.WriteBuffer(bs);
        }


        void SendTxtRequest(LiWsMsg wsMsgType, string str)
        {
            if (wsTxBuffer == null)
                return;
            byte[] bs = UTF8Encoding.UTF8.GetBytes(str);
            DataWriter w = new DataWriter(4 + 4 + bs.Length);
            w.WriteData(WsService.request_instance);
            w.WriteData((ushort)wsMsgType);
            w.WriteData(bs.Length);
            w.WriteData(bs);

            wsTxBuffer.WriteBuffer(w.GetBuffer());
        }
    }
}
