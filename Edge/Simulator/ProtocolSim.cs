using OpenWLS.Edge.Simulator.PLT1;
using OpenWLS.PLT1.Edge;
using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Edge.Simulator
{
    public interface IProtocolInst
    {
        ProtocolSim GetProtocolSim();
    }
    public  class ProtocolSim
    {
        protected InstrumentSims insts;
        protected Simulator simultor;

        public InstrumentSims Insts { set { insts = value; } }
        public virtual void ProceInstMsg(DataReader r)
        {

        }
    }
}
