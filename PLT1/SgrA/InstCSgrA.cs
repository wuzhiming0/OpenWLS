using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using OpenWLS.Client.LogInstance.Instrument;
using OpenWLS.PLT1.Edge;

namespace OpenWLS.PLT1.SgrA
{
    public class InstCSgrA : PLT1InstrumentC
    {
        public InstCSgrA() { Address = default_addr = IBProtocol.SGR_ADDR; }
        public override void CreateCntl()
        {
            cntl = new SgrACntl();
            cntl.Name = Name;
            ((SgrACntl)cntl).Inst = this;

            displays.Add(new SpecDisplay()
            {
                Name = $"Spec_{cntl.Name}",
            });
        }
    }
}
