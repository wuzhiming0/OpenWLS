using OpenWLS.PLT1.Edge;
using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Edge.Simulator.PLT1.ApA
{
    public class SimApA : PLT1InstrumentSim, IProtocolInst
    {

        ModuleDepth depth_mod;
        ModuleModem modem_mod;
        ModuleAdc adc_mod;
        PLT1InstModules mods;
        PLT1ProtocolSim protocol;
        public  SimApA()
        {
            int i = 1;
            modem_mod = new ModuleModem() { Id = i++ };
            adc_mod = new ModuleAdc() { Id = i++ };
            depth_mod = new ModuleDepth() { Id = i++ };
            mods = new PLT1InstModules() { modem_mod, adc_mod, depth_mod};
            EdgeDevice = true;
            protocol = new PLT1ProtocolSim();
            default_addr = IBProtocol.S_MOD_ADDR;
        }

        public override byte[] GetMGroup(int mg_id)
        {
            switch (mg_id)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
            return null;
        }


        public ProtocolSim GetProtocolSim()
        {
            return protocol;
        }
    }
}
