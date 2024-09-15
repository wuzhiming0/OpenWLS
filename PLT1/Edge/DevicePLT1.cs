using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWLS.Server.LogInstance.Edge;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogInstance;
using System.Reflection.Metadata.Ecma335;
using OpenWLS.Server.LogInstance.Instrument;

namespace OpenWLS.PLT1.Edge
{

    public class DevicePLT1 : Device
    {
        protected uint[]? portsInfor;
        protected uint? asset;
        public uint? Asset {
            get { return asset; } 
            set {   asset = value; }
        } 

        public DevicePLT1()
        {
            asset = null;
        }

        public override byte[]? GetDeviceInforBytes()
        {
            if (portsInfor == null)
                return null;
            else
                return IntArrayConverter.GetBytes(portsInfor);
        }

        protected override void ProcessPortList(byte[]? bs)
        {
            portsInfor = null;
            if (bs != null)
            {
                portsInfor = new uint[bs.Length >> 2];
                Buffer.BlockCopy(bs, 0, portsInfor, 0, bs.Length);
            }
        }

        public override void ProcMsgFromDevice(byte[]? bs)
        {
            ProcessPortList(bs);
        }

        public override void ProcMsgFromInsts(byte[]? bs)
        {
            if (bs == null) return;
            DataReader r = new DataReader(bs);
            Frame f = Frame.ReadFrame(r);
            Instrument? instr = logInstance.Insts.Find(a=>a.Address == f.SrcAddress);
            if(instr != null)
                ((PLT1Instrument)instr).ProcFrameFromInst(f);
        }

        public override void Close()
        {
            base.Close();
        }


    }
}
