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

namespace OpenWLS.PLT1.SgrA
{
    public class InstSgrA: PLT1Instrument
    {
        public InstSgrA()
        {
            Address = default_addr = IBProtocol.SGR_ADDR;
        }


        override public void ProcessParameter(string name, string value)
        {

        }

        protected override void InitCV(CVInstrument cvInst)
        {
            base.InitCV(cvInst);
            cvInst.GetSensor("GSP").GetCVPhase("CP").Tasks = new CPTasks();
            cvInst.GetSensor("GSP").GetCVPhase("VP").Tasks = new VPTasks();
       //     cvInst.GetSensor("GSP").GetCVPhase("VA").Tasks = new VATasks();
       //     cvInst.GetSensor("GSP").GetCVPhase("VB").Tasks = new VBTasks();

        }




        void ProcessExtCmdResponse(DataReader r)
        {
           

        }

        override public void OnConnected()
        {
      //      api.HwPort.SendExtendCmd(Address, (byte)SgrAExtCmdCde.GetEFInfor);
      //      api.HwPort.SendExtendCmd(Address, (byte)SgrAExtCmdCde.GetBBS);
      //      api.HwPort.SendExtendCmd(Address, (byte)SgrAExtCmdCde.GetDFiles);
        }     

    }
}
