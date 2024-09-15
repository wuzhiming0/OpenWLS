using OpenWLS.PLT1.Edge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Edge.Simulator.PLT1.MemA
{
    public class SimMemA : PLT1InstrumentSim
    {
        public SimMemA() { default_addr = IBProtocol.MEM_ADDR;}
        
    }
}
