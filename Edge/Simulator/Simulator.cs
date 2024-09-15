using OpenWLS.Server.Base;
using OpenWLS.Server.LogInstance.Edge;
using OpenWLS.Server.LogInstance.OperationDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static OpenWLS.PLT1.ApA.InstApA;

namespace OpenWLS.Edge.Simulator
{
    public class Simulator : Device
    {
        static void CreateInstrument(Simulator simulator, InstrumentOd inst_od)
        {
            InstrumentSim? inst = InstrumentSim.CreateSimInstrument(simulator, inst_od);
            if (inst == null)
                EdgeServer.WriteLine($"Failed to create {inst_od.Name}.");
            else
            {
                simulator.insts.Add(inst);
                EdgeServer.WriteLine($"Created {inst_od.Name}.");
            }
        }
        public static Simulator? CreateSimulator(string doc_json)
        {
            OperationDocument? doc = Newtonsoft.Json.JsonConvert.DeserializeObject<OperationDocument>(doc_json);
            if (doc == null)
            {
                return null;
            }
            Simulator simulator = new Simulator();
            simulator.opDoc = doc;
            foreach (InstrumentOd inst_od in doc.DhTools.Insts)
                CreateInstrument(simulator, inst_od);
            foreach (InstrumentOd inst_od in doc.SfEquipment.Insts)
                CreateInstrument(simulator, inst_od);
            InstrumentSim? edgeDevInst = simulator.insts.Where(a => a.EdgeDevice != null).FirstOrDefault();
            if (edgeDevInst != null)
            {
                simulator.protocol = ((IProtocolInst)edgeDevInst).GetProtocolSim();
                simulator.protocol.Insts = simulator.insts;
            }
            return simulator;
        }
        OperationDocument? opDoc;
        public OperationDocument? OpDoc
        {
            get { return opDoc; }
            set {
                opDoc = value;
             /*   Panels = new PanelInfors();
                foreach(InstSub s in opDoc.SfEquipment.Subs)
                {
                    PanelInfor pi = new PanelInfor()
                    { Model = s.Model, Asset = s.Asset };
                    Panels.Add(pi);
                }*/
            }
        }

        InstrumentSims insts;
        ProtocolSim? protocol;
        public Simulator() 
        {
            insts = new InstrumentSims();
            Type = DeviceType.Simulator;
        }

        public override void ProcDeviceMsg(byte[]? bs)
        {

        }

        public override void ProcInstMsg(byte[]? bs)
        {
            if (bs == null || protocol == null) return;
            protocol.ProceInstMsg(new DataReader(bs));
        }


    }
}
