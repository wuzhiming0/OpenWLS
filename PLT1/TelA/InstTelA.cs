using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Windows.Media;

using Newtonsoft.Json;

using OpenWLS.PLT1.Edge;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogInstance.Calibration;

namespace OpenWLS.PLT1.TelA
{
    public class InstTelA: PLT1Instrument
    { 
        public InstTelA()
        {
            Address = default_addr = IBProtocol.D_TEL_ADDR;
        }

       

        override public void ProcessParameter(string name, string value)
        {

        }

        protected override void InitCV(CVInstrument cvInst)
        {


        }




        void ProcessExtCmdResponse(DataReader r)
        {
            /*
            SgrAExtCmdCde ec = (SgrAExtCmdCde)r.ReadByte();
            switch (ec)
            {
                case SgrAExtCmdCde.HvGain:
                    break;
                case SgrAExtCmdCde.SignalInput:
                    break;
                case SgrAExtCmdCde.TestPulseWidth:
                    break;

               default:
                    break;
            } 
            */
        }

        /*
        override public void ProcessCmdResponse(byte cmd, DataReader r)
        {
            InstCommandCode cmdCode = (InstCommandCode)cmd;
            switch (cmdCode)
            {
                case InstCommandCode.INST_EXTAND:
                    ProcessExtCmdResponse(r);
                    break;
                default:
                    break;
            }
        }
        */
        override public void OnConnected()
        {
      //      api.HwPort.SendExtendCmd(Address, (byte)SgrAExtCmdCde.GetEFInfor);
      //      api.HwPort.SendExtendCmd(Address, (byte)SgrAExtCmdCde.GetBBS);
      //      api.HwPort.SendExtendCmd(Address, (byte)SgrAExtCmdCde.GetDFiles);
        }     

    }
}
