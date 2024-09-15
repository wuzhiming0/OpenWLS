using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using OpenWLS.Client.LogInstance.Instrument;
using OpenWLS.PLT1.Edge;

namespace OpenWLS.PLT1.TelA
{
    public class InstCTelA : PLT1InstrumentC
    {
        public InstCTelA()
        {
            Address = default_addr = IBProtocol.D_TEL_ADDR;
        }
        public override void CreateCntl()
        {
            cntl = new TelACntl();
            cntl.Name = Name;
            ((TelACntl)cntl).Inst = this;
        }

        public override void ResetInst()
        {
           // ((GInstMemACntl)cntl).ResetStatus();
        }
/*
        public override int ProcTxtMsg(DataReader r)
        {
            string str = r.ReadLine();
  
            return -2;
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
