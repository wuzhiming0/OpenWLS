using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using OpenWLS.Client.LogInstance.Instrument;
using OpenWLS.PLT1.Edge;
using OpenWLS.Server.Base;

namespace OpenWLS.PLT1.MemA
{


    public class InstCMemA : PLT1InstrumentC
    {
        public InstCMemA()
        {
            Address = default_addr = IBProtocol.MEM_ADDR;
            //        infor = new InstMem1Infor();
        }
        public override void CreateCntl()
        {
            cntl = new MemACntl();
            cntl.Name = Name;
            ((MemACntl)cntl).Inst = this;
        }

    /*    public override void InitInst()
        {
            ((GInstMemACntl)cntl).actCntl.ACT = ((APIClient.APIClient)Client).ACT;
        }
*/
        public override void ResetInst()
        {
            ((MemACntl)cntl).ResetStatus();
        }

        public  string ProcTxtMsg(DataReader r)
        {
            string str = r.ReadLine();
            if (str == "Usage")
            {
                ((MemACntl)cntl).SetEFlashUseage(r.ReadString());
             //   infor = JsonConvert.DeserializeObject<InstMem1Infor>(str);
                return null;
            }

            if (str == "Busy")
            {
                ((MemACntl)cntl).StartEFlashTask();
                //   infor = JsonConvert.DeserializeObject<InstMem1Infor>(str);
                return null;
            }

            if (str == "Ready")
            {
                ((MemACntl)cntl).EndEFlashTask();
                //   infor = JsonConvert.DeserializeObject<InstMem1Infor>(str);
                return null;
            }

            if (str == "Scan Error")
            {
                ((MemACntl)cntl).EndEFlashScanWithError();
                return null;
            }

            return "";
        }
    }
}
