using OpenWLS.PLT1.Edge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Edge.Simulator.PLT1.TelA
{
    public class SimTelA : PLT1InstrumentSim
    {
        public SimTelA() { default_addr = IBProtocol.D_TEL_ADDR; }
    }
}
