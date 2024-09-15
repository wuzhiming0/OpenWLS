using OpenWLS.PLT1.Edge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Edge.Simulator.PLT1.SgrA
{
    public class SimSgrA : PLT1InstrumentSim
    {
        public SimSgrA() { default_addr = IBProtocol.SGR_ADDR; }
    }
}
