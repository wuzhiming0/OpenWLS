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

namespace OpenWLS.PLT1.PtcA
{
    public class InstPtcA: PLT1Instrument
    {
        public InstPtcA()
        {
            Address = default_addr = IBProtocol.PTC_ADDR;
        }

        public string ProcGUIText(DataReader r)
        {
            string str = r.ReadLine();
            /*
            if (str == "Scan")
            {
                api.HwPort.SendExtendCmd(Address, (byte)SgrAExtCmdCde.Scan);
                api.SysLog.AddMessage(name_ext + " Scan", Colors.Blue);
                return 0;
            }
            */
            return null;
        }

        override public void ProcessParameter(string name, string value)
        {

        }

        protected override void InitCV(CVInstrument cvInst)
        {
            base.InitCV(cvInst);
         /*   cvInst.GetSensor("GR").GetCVPhase("CP").Tasks = new CPTasks();
            cvInst.GetSensor("GR").GetCVPhase("VP").Tasks = new VPTasks();
            cvInst.GetSensor("GR").GetCVPhase("VA").Tasks = new VATasks();
            cvInst.GetSensor("GR").GetCVPhase("VB").Tasks = new VBTasks();
*/
        }




        void ProcessExtCmdResponse(DataReader r)
        {
            /*
            GrAExtCmdCde ec = (GrAExtCmdCde)r.ReadByte();
            switch (ec)
            {
                case GrAExtCmdCde.HvGain:
                    break;
                case GrAExtCmdCde.SignalInput:
                    break;
                case GrAExtCmdCde.TestPulseWidth:
                    break;

               default:
                    break;
            } 
            */
        }



        override public void OnConnected()
        {
      //      api.HwPort.SendExtendCmd(Address, (byte)SgrAExtCmdCde.GetEFInfor);
      //      api.HwPort.SendExtendCmd(Address, (byte)SgrAExtCmdCde.GetBBS);
      //      api.HwPort.SendExtendCmd(Address, (byte)SgrAExtCmdCde.GetDFiles);
        }     

    }
}
