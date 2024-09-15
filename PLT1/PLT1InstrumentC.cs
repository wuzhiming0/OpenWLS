using OpenWLS.Client.LogInstance;
using OpenWLS.Client.LogInstance.Instrument;
using OpenWLS.PLT1.Edge;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogInstance.Instrument;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenWLS.PLT1
{
    public  class PLT1InstrumentC :InstrumentC
    {
        protected byte default_addr;
        [JsonIgnore]
        public byte DefaultAddr { get { return default_addr; } }  //perfferred logical ID
        [JsonIgnore]
        public uint? Asset
        {
            get {
                if (Subs == null || Subs.Count == 0 || Subs[0].Asset == null) return null;
                else return Convert.ToUInt32(Subs[0].Asset);
            }
            set {
                if (Subs == null || Subs.Count == 0 || Subs[0].Asset == null) return;
                Subs[0].Asset = value.ToString();
            }
        }
        protected PLT1InstGenInfor? genInfor;
        protected InstCntlTbl? cntlTbl;

        [JsonIgnore]
        public InstCntlTbl? CntlTbl { get { return cntlTbl; } set { cntlTbl = value; } }
        protected virtual void OnCntlTblChanged()
        {

        }

        protected virtual void OnCntlTblItemChanged(ushort offset, int size)
        {

        }
        void ProcCntlTblItem(byte[] bs)
        {
            DataReader r = new DataReader(bs);
            ushort offset = r.ReadUInt16();
            int size = bs.Length - 2;
            if (cntlTbl != null)
            {
                cntlTbl.SetItemBytes(offset, r.ReadByteArray(size));
                OnCntlTblItemChanged(offset, size);
            }
        }
        protected virtual void ProcessOtherInstCntlBlock(Block b)
        {

        }
        protected override void ProcInstCntlGuiMsg(byte[]? bs)
        {
            if (bs == null) return;
            Frame f = Frame.ReadFrame(new DataReader(bs));
            foreach(Block b in f.Blocks)
            {
                switch (b.Type)
                {
                    case (byte)PLT1InstMsgCode.GENERAL_INFOR:
                        if(b.Body != null)
                            SetGenInfor(b.Body);
                        break;
                    case (byte)PLT1InstMsgCode.INST_CNTL_TBL:
                        if (cntlTbl != null && b.Body != null)
                        {
                            cntlTbl.Restore(b.Body); 
                            OnCntlTblChanged();
                        }
                        break;
                    case (byte)PLT1InstMsgCode.INST_CNTL_ITEM:
                        if (b.Body != null)
                            ProcCntlTblItem(b.Body);
                        break;

                    default:
                        ProcessOtherInstCntlBlock(b);
                        break;
                }
            }
        }
        public void SetGenInfor(byte[] bs)
        {
            genInfor = new PLT1InstGenInfor();
            genInfor.Restore(bs);
            Address = genInfor.Addr;
            genInfor.UpdateAssetOfSubs(Subs);
            Verified = true;
            OnPropertyChanged("InstName");
            OnPropertyChanged("Color");
        }
        public void SendInstMsg(byte cmd)
        {
            if (Address != null)
            {
                byte[] bs = Frame.GetFrameBytes((byte)Address, IBProtocol.OpenWLS_ADDR, cmd, true);
                liClient.SendInstGuiPackage(Id, (ushort)InstGuiMsgType.InstCntl, bs);
            }
        }
        public void SendInstMsg(byte cmd, bool bt)
        {
            if (Address != null)
            {
                byte[] bs = Frame.GetFrameBytes((byte)Address, IBProtocol.OpenWLS_ADDR, cmd, bt);
                liClient.SendInstGuiPackage(Id, (ushort)InstGuiMsgType.InstCntl, bs);
            }
        }
        public void SendInstMsg(byte inst_addr, byte cmd, bool bt)
        {
            byte[] bs = Frame.GetFrameBytes(inst_addr, IBProtocol.OpenWLS_ADDR, cmd, bt);
            liClient.SendInstGuiPackage(Id, (ushort)InstGuiMsgType.InstCntl, bs);
        }
        public void SendInstMsg( byte inst_addr, byte cmd)
        {
            SendInstMsg(inst_addr, cmd, true);
        }
        public void SendInstMsg( byte inst_addr, byte cmd, byte[]? body, bool bt)
        {
            byte[] bs = Frame.GetFrameBytes(inst_addr, IBProtocol.OpenWLS_ADDR, cmd, body, bt);
            liClient.SendInstGuiPackage(Id, (ushort)InstGuiMsgType.InstCntl, bs);
        }
        /*
        public void SendEdgeDeviceMsg(  byte[]? body, bool bt)
        {
            liClient.SendInstGuiPackage(Id, (ushort)InstGuiMsgType.EdgeDevice, body);
        }*/

        public void SendInstMsg( byte inst_addr, byte cmd, byte[]? body)
        {
            SendInstMsg(inst_addr, cmd, body, true);
        }
        public void SendInstMsg( byte cmd, byte[]? body)
        {
            if (Address != null)
                SendInstMsg((byte)Address, cmd, body, true);
        }


        public void SendCntTblItem(ushort offset, byte[] bs)
        {
            if (Address != null && cntlTbl != null)
                SendInstMsg((byte)PLT1InstMsgCode.INST_CNTL_ITEM, InstCntlTbl.GetItemBodyBytes(offset, bs));
        }
        public void SendCntTblItem(ushort offset, ushort size)
        {
            if(Address != null && cntlTbl != null)
                SendInstMsg((byte)PLT1InstMsgCode.INST_CNTL_ITEM, cntlTbl.GetItemBodyBytes(offset, size));
        }
        public void SendCntTbl()
        {
            if (Address != null && cntlTbl != null)
                SendInstMsg((byte)PLT1InstMsgCode.INST_CNTL_TBL, cntlTbl.GetTotalBytes());
        }

    }
}
