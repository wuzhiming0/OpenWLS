using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWLS.PLT1.Edge;
using Newtonsoft.Json;
using OpenWLS.PLT1;

namespace OpenWLS.Edge.Simulator.PLT1
{
    public class PLT1InstrumentSim : InstrumentSim
    {
        protected uint asset_nu;
        protected uint model_nu;

        protected byte default_addr;
        protected Frame package_proc;
        [JsonIgnore]
        public byte DefaultAddr { get { return default_addr; } }  //perfferred logical ID

        public void ProceDlinkBlocks(Frame p)
        {
            package_proc = p;
            foreach (var b in p.Blocks)
            {
                PLT1InstMsgCode m_code = (PLT1InstMsgCode)(b.Type & IBProtocol.MSG_CODE_MASK);
                bool read = (b.Type & IBProtocol.MSG_READ_MASK) != 0;
                switch (m_code)
                {
                    case PLT1InstMsgCode.RESET_ADDR:
                        Address = default_addr;
                        EdgeServer.WriteLine($"{Name}: set address to default {default_addr}.");
                        break;
                    case PLT1InstMsgCode.GENERAL_INFOR:
                        Id = BitConverter.ToInt32(b.Body);
                        if(read)
                            SendGenInfor();
                        break;

                    default:
                        ProcDLinkSpecialBlock(m_code, read);
                        break;
                }
            }
        }
        public virtual void ProcDLinkSpecialBlock(PLT1InstMsgCode m_code, bool read)
        {

        }

        void SendGenInfor()
        {
            byte[] bs = PLT1InstGenInfor.GetBytes(this);
            byte[] bs1 = Frame.GetFrameBytes(package_proc.SrcAddress, (byte)Address, (byte)PLT1InstMsgCode.GENERAL_INFOR, bs, true);
            simulator.SendPackageFromInst(bs1);
            EdgeServer.WriteLine($"{Name}: Get GenInfor.  Addr={Address}, asset={asset_nu}, model={model_nu}.");
        }

    }
}
