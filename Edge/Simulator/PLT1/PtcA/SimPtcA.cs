using OpenWLS.PLT1.Edge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Edge.Simulator.PLT1.PtcA
{
    public class SimPtcA : PLT1InstrumentSim
    {
        public SimPtcA() { default_addr = IBProtocol.PTC_ADDR; }
    }
}
