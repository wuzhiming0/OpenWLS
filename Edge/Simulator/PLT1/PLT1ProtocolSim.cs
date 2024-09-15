using OpenWLS.Server.Base;
//using OpenWLS.Server.LogInstance.Edge;
using OpenWLS.PLT1.Edge;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWLS.Server.LogInstance.OperationDocument;

namespace OpenWLS.Edge.Simulator.PLT1
{
    public class PLT1ProtocolSim : ProtocolSim
    {
        public override void ProceInstMsg(DataReader r)
        {
            Frame p = Frame.ReadFrame(r);
            if (p.DstAddress == IBProtocol.GLOBAL_ADDR)
            {
                foreach (PLT1InstrumentSim inst in insts)
                    inst.ProceDlinkBlocks(p);
            }
            else
            {
                InstrumentSim? inst = insts.Where(a => a.Address == p.DstAddress).FirstOrDefault();
                if (inst != null)
                    ((PLT1InstrumentSim)inst).ProceDlinkBlocks(p);
            }
        }

        
    }
}
