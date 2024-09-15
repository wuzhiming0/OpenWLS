using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using OpenWLS.Client.LogInstance.Instrument;
using OpenWLS.PLT1.Edge;

namespace OpenWLS.PLT1.PtcA
{
    public class InstCPtcA : PLT1InstrumentC
    {
        public InstCPtcA() { Address = default_addr = IBProtocol.PTC_ADDR; }

        public override void CreateCntl()
        {
            cntl = new PtcACntl();
            cntl.Name = Name;
            ((PtcACntl)cntl).Inst = this;

         //   displays.Add(new SpecDisplay());
        }
    }
}
