using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Windows.Media;

using Newtonsoft.Json;
using OpenWLS.Server.LogInstance.Instrument;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogInstance.Calibration;
using OpenWLS.Server.LogInstance.Edge;
using OpenWLS.PLT1.Edge;
using OpenWLS.Server.DBase.Models.LocalDb;
using OpenWLS.Server.LogInstance;
using System.Security.Cryptography.X509Certificates;

namespace OpenWLS.PLT1.ApA
{
    //Acquisition Pannel
    public class InstApA : PLT1Instrument, IEdgeDevice
    {
        public enum InstCmdCode {  SetDepth = 0x40,   StartTrain = 0x41, StopTrain = 0x42 };


        DevPLT1Ap edgeDev;

        public InstApA()
        {
            edgeDev = new DevPLT1Ap();
            edgeDev.Instrument = this;
            Address = default_addr = IBProtocol.S_MOD_ADDR;
        }
/*
        public  void SendPortInforsToClient()
        {
            byte[]? bs = edgeDev.PortAssets == null? null: IntArrayConverter.GetBytes(edgeDev.PortAssets);
         //   byte[] bs2 = Frame.GetFrameBytes(IBProtocol.OpenWLS_ADDR, (byte)IBProtocol.S_MOD_ADDR, (byte)InstApA.InstCmdCode.PortInfor, bs1, true);
            ((ushort)InstGuiMsgType.EdgeDevice, bs);
        }
*/
        public override void Init(LogInstanceS li)
        {
            base.Init(li);
        }
        public Device GetDevice()
        {
            return edgeDev;
        }
        public bool? IsToolPowerOn()
        {
            return true;
        }

        public bool IsCompatible(Server.DBase.Models.LocalDb.Edge edge)
        {
            return edge.EType == null || edge.EType == (int)EdgeType.OpenWLS_Std;
        }

        protected override void ProcInstCntlGuiMsg(byte[] bs)
        {
            //base.ProcInstCntlGuiMsg(bs);
        }

        override public void ProcessParameter(string name, string value)
        {

        }

        protected override void InitCV(CVInstrument cvInst)
        {


        }





        override public void OnConnected()
        {
            //      api.HwPort.SendExtendCmd(Address, (byte)SgrAExtCmdCde.GetEFInfor);
            //      api.HwPort.SendExtendCmd(Address, (byte)SgrAExtCmdCde.GetBBS);
            //      api.HwPort.SendExtendCmd(Address, (byte)SgrAExtCmdCde.GetDFiles);
        }

        override protected void OnStartLog(bool depthAsIndex, bool record, double? from, double? to)
        {

        }


    }
}
/*
 *


v
*/