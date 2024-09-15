using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Newtonsoft.Json;
using OpenWLS.Client.LogInstance.Instrument;
using OpenWLS.PLT1.Edge;

namespace OpenWLS.PLT1.GrA
{
    public class InstCGrA : PLT1InstrumentC
    {
        public InstCGrA() { Address = default_addr = IBProtocol.GR_ADDR; }
        public override void CreateCntl()
        {
            cntl = new GrACntl();
            cntl.Name = Name;
            ((GrACntl)cntl).Inst = this;
           
         //   displays.Add(new SpecDisplay());
        }
    }
}
