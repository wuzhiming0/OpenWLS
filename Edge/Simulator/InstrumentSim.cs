using OpenWLS.Server.Base;
using OpenWLS.Server.LogInstance.OperationDocument;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Edge.Simulator
{
    public  class InstrumentSim : InstrumentOd
    {
        public bool? EdgeDevice { get; set; }

        protected Simulator simulator;
        AcquisitionEngine ae;
        public static InstrumentSim? CreateSimInstrument(Simulator sim, InstrumentOd inst_od)
        {
            string n1 = $"OpenWLS.Edge.Simulator.{inst_od.Category}.{inst_od}.Sim{inst_od.Name}";
            InstrumentSim inst = (InstrumentSim)DataType.CreatObject(n1, "Edge.dll");
            if (inst == null) return null;
            inst.simulator = sim;
            inst.Id = inst_od.Id;
            inst.Address = inst_od.Address;
            inst.Name = inst_od.Name;
            inst.SurfaceEqu = inst_od.SurfaceEqu;

            InstSubs ss = inst.SurfaceEqu == null ? sim.OpDoc.DhTools.Subs : sim.OpDoc.SfEquipment.Subs;
            inst.Subs = new InstSubs(ss.Where(a => a.IId == inst.Id).ToList());
            inst.ae = new AcquisitionEngine(inst);
            inst.ae.AcqItems = AcqItems.GetAcqItemsOfInst(sim.OpDoc.ACT, inst.Id);
            return inst;
        }

        public virtual byte[]? GetMGroup(int mg_id)
        {
            return null;
        }

        public virtual void AcquireMgroup(int mg)
        {

        }

    /*    public virtual void ProcDLinkMsg(int m_type, byte[]? m_body)
        {
        }
    */
    }

    public class InstrumentSims : List<InstrumentSim>
    {

    }


}
