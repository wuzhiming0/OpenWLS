using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using OpenWLS.Client.LogInstance.Instrument;
using OpenWLS.PLT1.Edge;

namespace OpenWLS.PLT1.TesterA
{


    public class InstCTesterA : PLT1InstrumentC
    {
        public InstCTesterA()
        {
            Address = default_addr = IBProtocol.S_MOD_ADDR;
            //        infor = new InstMem1Infor();
        }
        public override void CreateCntl()
        {
            cntl = new TesterACntl();
            cntl.Name = Name;
            ((TesterACntl)cntl).Inst = this;
        }

    /*    public override void InitInst()
        {
            ((GInstMemACntl)cntl).actCntl.ACT = ((APIClient.APIClient)Client).ACT;
        }
*/
        public override void ResetInst()
        {
           // ((GInstMemACntl)cntl).ResetStatus();
        }



/*        public override string ProcTxtMsg(string msg)
        {
            string str = r.ReadLine();
  
            return "";
        }
        private void powerBtn_Click(object sender, RoutedEventArgs e)
        {
            string str = PowerOn ? "nOff" : "\nOn";
            gui.SendRequest(LogInstance.str_api_req_tpower + str);
        }

        private void scanToolBtn_Click(object sender, RoutedEventArgs e)
        {
            //apiGui.SendRequest("mainTBar\nScan\n");
        }*/
    }
}
